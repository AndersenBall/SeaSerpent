using UnityEngine;

[DisallowMultipleComponent]
public class BoatSteeringControls : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private TargetMarker targetMarker;


    [Header("Steering (PD)")]
    [SerializeField] private float Kp = 2.0f;
    [SerializeField] private float Kd = 0.25f;

    [Header("Arrival/Braking")]
    [Tooltip("Start slowing down when within this distance to the target (meters).")]
    [SerializeField] private float slowDownRadius = 20f;
    [Tooltip("Consider 'arrived' and stop within this distance (meters).")]
    [SerializeField] private float stopDistance = 2f;
    
    [Header("Wiring")]
    [SerializeField] private BoatControls boatControls;
    
    [Header("Circle Mode")]
    [Tooltip("If enabled, the boat will circle the target, maintaining a perfect 90° angle to it.")]

    private bool _circle = false;
    public bool circle { get => _circle; set => _circle = value; }
    [Tooltip("When circling, choose direction: true = clockwise, false = counter-clockwise.")]
    private bool _circleClockwise = true;
    public bool CircleClockwise { get => _circleClockwise; set => _circleClockwise = value; }

    
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (!boatControls) boatControls = GetComponent<BoatControls>();
    }

    public void SetTargetMarker(TargetMarker marker)
    {
        targetMarker = marker;
    }
    
    public void SetTargetPosition(Vector3 worldPosition)
    {
        targetMarker.SetPosition(worldPosition);
    }


    private void FixedUpdate()
    {
        ComputeSteeringXZ(out float steer, out float throttle);
        boatControls.SetTurn(steer);
        boatControls.SetForward(throttle);
    }

    private void ComputeSteeringXZ(out float steerOut, out float throttleOut)
    {
        steerOut = 0f;
        throttleOut = 1f; 

        if (!targetMarker) return;

        // --- positions on XZ ---
        Vector2 self = new Vector2(transform.position.x, transform.position.z);
        Vector2 goal = new Vector2(targetMarker.transform.position.x,   targetMarker.transform.position.z);

        Vector2 toTgt = goal - self;
        float d = toTgt.magnitude;
        if (d <= stopDistance){
            throttleOut = 0f;
            return;
        }

        Vector2 toTgtN = toTgt / d;

        // --- forward on XZ using your actual transform ---
        Vector3 fwd3 = transform.forward;                    
        Vector2 fwd  = new Vector2(fwd3.x, fwd3.z).normalized;
        float angVelY = transform.InverseTransformDirection(_rb.angularVelocity).y;
        
        if (circle)
        {
            // Tangent direction = 90° from the target vector (perfect perpendicular)
            // Clockwise uses +90° rotation on the XZ plane (y,-x); CCW uses -90° (-y,x).
            Vector2 tangent = CircleClockwise
                ? new Vector2(toTgtN.y, -toTgtN.x)   // CW
                : new Vector2(-toTgtN.y, toTgtN.x);  // CCW

            // signed angle from forward -> tangent on XZ
            float dot = Mathf.Clamp(Vector2.Dot(fwd, tangent), -1f, 1f);
            float perp = fwd.x * tangent.y - fwd.y * tangent.x; // + = left (CCW)
            float angleErr = Mathf.Atan2(perp, dot);            // radians [-pi, pi]

            // PD steer toward tangent (90° to target line)
            steerOut = Mathf.Clamp(Kp * angleErr - Kd * angVelY, -1f, 1f);

            // Keep moving; reduce throttle a bit when turning hard
            float turnSlowdown = Mathf.Lerp(1f, 0.5f, Mathf.InverseLerp(0.3f, 1f, Mathf.Abs(steerOut)));
            throttleOut = Mathf.Clamp01(turnSlowdown);
        }
        else
        {
            // Normal: steer directly toward the target
            // signed angle from forward -> target on XZ
            float dot = Mathf.Clamp(Vector2.Dot(fwd, toTgtN), -1f, 1f);
            float perp = fwd.x * toTgtN.y - fwd.y * toTgtN.x;  // + = left (CCW)
            float angleErr = Mathf.Atan2(perp, dot);           // radians [-pi, pi]

            // PD steer (tune gains to taste)
            steerOut = Mathf.Clamp(Kp * angleErr - Kd * angVelY, -1f, 1f);

            if (d <= stopDistance)
            {
                throttleOut = 0f;
                return;
            }

            if (d <= slowDownRadius)
            {
                throttleOut = 0f;
            }
            else
            {
                // Reduce throttle a bit when turning hard
                float turnSlowdown = Mathf.Lerp(1f, 0.5f, Mathf.InverseLerp(0.3f, 1f, Mathf.Abs(steerOut)));
                throttleOut = Mathf.Clamp01(turnSlowdown);
            }
        }

    }

}
