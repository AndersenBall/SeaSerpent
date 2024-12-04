using UnityEngine;


public sealed class MapCamera : MonoBehaviour
{
    #region PUBLIC FIELDS

    [SerializeField]
    private Transform _targetTransform;

    [SerializeField]
    private float _distanceToTarget = 50.0f;

    [SerializeField]
    private float _followSpeed = 3.0f;

    [SerializeField]
    private float _freeMoveSpeed = 50.0f;

    [SerializeField]
    private float _scrollSpeed = 100.0f;

    [SerializeField]
    private float _minZoomDistance = 50.0f; 
    [SerializeField]
    private float _maxZoomDistance = 1000.0f;

    [SerializeField]
    private Camera _camera;

    private float initialHeight;

    #endregion

    #region PROPERTIES

    public Transform targetTransform
    {
        get { return _targetTransform; }
        set { _targetTransform = value; }
    }

    public float distanceToTarget
    {
        get { return _distanceToTarget; }
        set { _distanceToTarget = Mathf.Clamp(value, _minZoomDistance, _maxZoomDistance); }
    }


    public float followSpeed
    {
        get { return _followSpeed; }
        set { _followSpeed = Mathf.Max(0.0f, value); }
    }

    private Vector3 cameraRelativePosition
    {
        get { return new Vector3(targetTransform.position.x, initialHeight, targetTransform.position.z); }
    }

    private bool isFollowing = true;

    #endregion

    #region MONOBEHAVIOUR

    public void OnValidate()
    {
        distanceToTarget = _distanceToTarget;
        followSpeed = _followSpeed;
    }

    public void Awake()
    {
        _camera = GetComponent<Camera>();
        initialHeight = transform.position.y;
    }

    public void LateUpdate()
    {
        if (isFollowing && targetTransform != null)
        {

            Vector3 targetPosition = new Vector3(
                Mathf.Lerp(transform.position.x, targetTransform.position.x, followSpeed * Time.deltaTime),
                initialHeight,
                Mathf.Lerp(transform.position.z, targetTransform.position.z, followSpeed * Time.deltaTime)
            );

            transform.position = targetPosition;
        }
        else
        {

            FreeCameraMovement();
        }

        HandleMouseInput();
        HandleScrollInput();
    }

    #endregion

    #region CUSTOM METHODS

    private void FreeCameraMovement()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector3.right;
        }

        transform.position += direction * (_freeMoveSpeed + _distanceToTarget) * Time.deltaTime;
    }

    private void HandleMouseInput()
    {
        int layerMask = (1 << 13) | (1 << 14); //team1 and team2
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
   
                    targetTransform = hit.transform;
                    isFollowing = true; 

            }
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            isFollowing = false;
        }
    }

    private void HandleScrollInput()
    {
        // Get the scroll wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            
            distanceToTarget -= scroll * _scrollSpeed;
            
            _camera.orthographicSize  = Mathf.Clamp(distanceToTarget, _minZoomDistance, _maxZoomDistance);
                
            
        }

  
    }

    #endregion
}
