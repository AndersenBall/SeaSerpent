using System;
using UnityEngine;

public enum AttackSide { Left, Right }


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
    [SerializeField] public float slowDownRadius = 20f;
    [Tooltip("Consider 'arrived' and stop within this distance (meters).")]
    [SerializeField] public float stopDistance = 10f;
    [SerializeField] private float _distanceToTarget;
    public float DistanceToTarget{get => _distanceToTarget; set => _distanceToTarget = value; }
    
    [Header("Wiring")]
    [SerializeField] private BoatControls boatControls;
    
    [Header("Circle Mode")] 
    [Tooltip("If enabled, the boat will circle the target, maintaining a perfect 90° angle to it.")]
    [SerializeField]private bool _circle = false;
    public bool circle { get => _circle; set => _circle = value; }
    
    [Tooltip("When circling, choose direction: true = clockwise, false = counter-clockwise.")]
    [SerializeField]private bool _circleClockwise = true;
    public bool CircleClockwise { get => _circleClockwise; set => _circleClockwise = value; }

    
    [Header("Collision Avoidance")]
    [SerializeField] private bool enableAvoidance = true;
    [SerializeField] private LayerMask shipLayer;
    [SerializeField] private float detectionRadius = 700f;   // meters
    [SerializeField] private float clearanceRadius = 30f;    // meters (hull + buffer)
    [SerializeField] private float timeHorizon = 5.0f;      // seconds to look ahead
    [SerializeField, Range(0f,1f)] private float avoidBlend = 1f; // weight of avoidance vs goal
    [SerializeField] private float keepRightBias = 0.0f;    // bias to pass starboard-to-starboard
    [SerializeField] private float hardBrakeTTC = 0.8f;     // emergency throttle cut threshold

    
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (!boatControls) boatControls = GetComponent<BoatControls>();
    }

    private void Update()
    {
        if (_distanceToTarget < stopDistance){
            targetMarker.AdvanceToNextWaypoint();
        }
    }

    public void SetTargetMarker(TargetMarker marker)
    {
        targetMarker = marker;
    }

    public void SetTargetMarketVisable(bool visible)
    {
        targetMarker.SetVisible(visible);
    }

    public void SetTargetPosition(int x , int z)
    {
        targetMarker.SetPosition(new Vector3(x, 10, z));
    }
    
    public void SetTargetPosition(Vector3 worldPosition)
    {
        targetMarker.SetPosition(new Vector3(worldPosition.x, 10, worldPosition.z));
    }

    public void SetTargetPosition(GameObject target)
    {
        targetMarker.FollowTarget(target);
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
        DistanceToTarget = d;
        if (d <= stopDistance){
            throttleOut = 0f;
            return;
        }

        Vector2 toTgtN = toTgt / d;

        // --- forward on XZ using your actual transform ---
        Vector3 fwd3 = transform.forward;                    
        Vector2 fwd  = new Vector2(fwd3.x, fwd3.z).normalized;
        float angVelY = transform.InverseTransformDirection(_rb.angularVelocity).y;
        
        Vector2 desiredDir;
        if (circle)
        {
            Vector2 tangent = CircleClockwise
                ? new Vector2(toTgtN.y, -toTgtN.x)
                : new Vector2(-toTgtN.y, toTgtN.x);
            desiredDir = tangent.normalized;
        }
        else
        {
            desiredDir = toTgtN;
        }

        bool imminent = false;
        if (enableAvoidance)
        {
            Vector2 velXZ = new Vector2(_rb.velocity.x, _rb.velocity.z);
            Vector2 avoidDir = ComputeAvoidance(self, velXZ, out imminent);
            if (avoidDir.sqrMagnitude > 0.0001f)
            {
                desiredDir = (avoidDir * avoidBlend + desiredDir * (1f - avoidBlend)).normalized;
                
                //Debug for avoidance
                Vector3 basePos = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
                Vector3 rawAvoid = new Vector3(avoidDir.x, 0f, avoidDir.y).normalized;
                Vector3 adjustedDir3 = new Vector3(desiredDir.x, 0f, desiredDir.y).normalized;
                Vector3 pureDir3     = new Vector3(toTgtN.x,    0f, toTgtN.y   ).normalized;
                // Draw 10m lines
                Debug.DrawLine(basePos, basePos + adjustedDir3 * 40f, Color.green);  
                Debug.DrawLine(basePos, basePos + pureDir3     * 40f, Color.yellow);
                Debug.DrawLine(basePos, basePos + rawAvoid     * 40f, Color.red);
            }
        }
        
        if (circle)
        {
            float dot = Mathf.Clamp(Vector2.Dot(fwd, desiredDir), -1f, 1f);
            float perp = fwd.x * desiredDir.y - fwd.y * desiredDir.x;
            float angleErr = Mathf.Atan2(perp, dot);

            steerOut = Mathf.Clamp(-1*Kp * angleErr - Kd * angVelY, -1f, 1f);

            float turnSlowdown = Mathf.Lerp(1f, 0.5f, Mathf.InverseLerp(0.3f, 1f, Mathf.Abs(steerOut)));
            throttleOut = Mathf.Clamp01(turnSlowdown);
        }
        else
        {
            float dot = Mathf.Clamp(Vector2.Dot(fwd, desiredDir), -1f, 1f);
            float perp = fwd.x * desiredDir.y - fwd.y * desiredDir.x;
            float angleErr = Mathf.Atan2(perp, dot);

            steerOut = Mathf.Clamp(-1*Kp * angleErr - Kd * angVelY, -1f, 1f);

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
                float turnSlowdown = Mathf.Lerp(1f, 0.5f, Mathf.InverseLerp(0.3f, 1f, Mathf.Abs(steerOut)));
                throttleOut = Mathf.Clamp01(turnSlowdown);
            }
        }
        if (enableAvoidance && imminent)
        {
            throttleOut = Mathf.Min(throttleOut, 0.25f);
        }

    }
    
    private Vector2 ComputeAvoidance(Vector2 selfPosXZ, Vector2 selfVelXZ, out bool imminent)
    {
        imminent = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, shipLayer, QueryTriggerInteraction.Ignore);
        if (hits.Length == 0) return Vector2.zero;

        Vector2 sum = Vector2.zero;
        float bestImminence = float.MaxValue;

        foreach (var h in hits)
        {
            if (h.attachedRigidbody == null || h.attachedRigidbody == _rb) continue;

            Vector3 otherPos3 = h.attachedRigidbody.position;
            Vector3 otherVel3 = h.attachedRigidbody.velocity;

            Vector2 otherPos = new Vector2(otherPos3.x, otherPos3.z);
            Vector2 otherVel = new Vector2(otherVel3.x, otherVel3.z);

            Vector2 r = otherPos - selfPosXZ;              // relative position
            Vector2 v = otherVel - selfVelXZ;              // relative velocity

            float v2 = v.sqrMagnitude;
            if (v2 < 0.5f) // ~0.5 m/s relative speed; tune as needed
            {
                float dist = r.magnitude;
                if (dist < clearanceRadius * 1.6f)
                {
                    // Deterministic right-of-way so we don't mirror each other
                    int myId    = _rb.GetInstanceID();
                    int otherId = h.attachedRigidbody.GetInstanceID();
                    bool iYield = myId > otherId; // one yields, one proceeds (consistent for the pair)

                    // "Away" is radial separation; "tangentRight" circles around their starboard side
                    Vector2 away          = (-r).normalized;
                    Vector2 tangentRight  = new Vector2(-away.y, away.x);   // right around them
                    Vector2 tangentLeft   = -tangentRight;                  // left around them

                    // Stand-on goes one way, give-way goes the other (breaks symmetry)
                    Vector2 chosenTangent = iYield ? tangentRight : tangentLeft;

                    // Make the move mostly tangential (to slide past), with a bit of radial spacing
                    float nearWeight = Mathf.InverseLerp(clearanceRadius * 1.6f, 0f, dist); // closer => stronger
                    Vector2 nudge = chosenTangent * (0.65f + 0.35f * nearWeight) + away * 0.25f;

                    sum += nudge.normalized * Mathf.Clamp01(nearWeight);
                    bestImminence = Mathf.Min(bestImminence, 0.25f); // treat as urgent so throttle clamps a bit
                }
                continue;
            }

            float tStar = Mathf.Clamp(-Vector2.Dot(r, v) / v2, 0f, timeHorizon);
            Vector2 rAtClosest = r + v * tStar;
            float sep = rAtClosest.magnitude;

            if (sep < clearanceRadius)
            {
                // --- choose side & prefer passing BEHIND the other ship ---

                Vector2 left  = new Vector2(v.y, -v.x).normalized;
                Vector2 right = -left;

                Vector2 fwdSelf  = new Vector2(transform.forward.x, transform.forward.z).normalized;
                Vector2 fwdOther = new Vector2(h.attachedRigidbody.transform.forward.x,
                                               h.attachedRigidbody.transform.forward.z).normalized;

                // Are we closing and is the other roughly in front of us?
                float closingSpeed = Vector2.Dot(v, -r);            // > 0 => closing
                bool  otherAhead   = Vector2.Dot(fwdSelf, r.normalized) > 0f;

                // Are we in FRONT of the other at CPA (>0) or BEHIND (<0)?
                float frontnessAtCPA = Vector2.Dot(fwdOther, rAtClosest);

                // Preview how a lateral nudge changes that frontness (which side tucks us behind more)
                float preview = Mathf.Max(5f, clearanceRadius * 0.5f);
                float frontnessIfLeft  = Vector2.Dot(fwdOther, rAtClosest + left  * preview);
                float frontnessIfRight = Vector2.Dot(fwdOther, rAtClosest + right * preview);

                // Natural side from current relative motion (stability)
                float sideSign = Mathf.Sign(fwdSelf.x * v.y - fwdSelf.y * v.x); // + => left is natural
                Vector2 chosen = sideSign >= 0f ? left : right;

                // If we’re closing, the other is in front, and CPA says we’d be in FRONT of them,
                // pick the side that makes us end up more BEHIND (frontness more negative).
                if (closingSpeed > 0f && otherAhead && frontnessAtCPA > 0f)
                {
                    chosen = (frontnessIfLeft <= frontnessIfRight) ? left : right;
                }
                else
                {
                    // Otherwise apply a small keep-right to break symmetry
                    chosen = Vector2.Lerp(chosen, right, keepRightBias).normalized;
                }

                // --- add a "tuck behind" pull so we slide into the gap opening behind them ---
                Vector2 otherVelXZ = otherVel; // already computed above
                float  otherSpeed  = otherVelXZ.magnitude;
                if (otherSpeed > 0.1f)
                {
                    // Aim a little bit behind their stern, with a small lateral offset toward 'chosen'
                    float followDist    = Mathf.Max(clearanceRadius * 1.5f, 10f);
                    float lateralOffset = Mathf.Max(clearanceRadius * 0.6f, 5f);

                    Vector2 tuckDir = (-fwdOther * followDist + chosen * lateralOffset).normalized;

                    // Blend the tuck with the pure side step: this favors slipping behind rather than nosing ahead
                    float tuckWeight = 0.6f; // tune 0.4–0.8
                    chosen = (chosen * (1f - tuckWeight) + tuckDir * tuckWeight).normalized;
                }

                // Weight by time-to-contact & separation as before
                float tWeight = 1f - (tStar / timeHorizon);
                float sWeight = 1f - Mathf.InverseLerp(0f, clearanceRadius, sep);
                float weight  = Mathf.Clamp01(0.5f * tWeight + 0.5f * sWeight);

                sum += chosen * weight;
                bestImminence = Mathf.Min(bestImminence, tStar);

            }
        }

        if (bestImminence < hardBrakeTTC) imminent = true;

        return sum.sqrMagnitude > 0.0001f ? sum.normalized : Vector2.zero;
    }
}
