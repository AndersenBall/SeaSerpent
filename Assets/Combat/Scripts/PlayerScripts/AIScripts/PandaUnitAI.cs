using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class PandaUnitAI : MonoBehaviour
{
    #region variables
    
    public string currentAction ;
    public bool BroadsideMode = false;
    
    public float fireSpeed;
    private bool hasCannonBall = false;
    [SerializeField]
    private float startTime;

    public GameObject cannonBallPrefab;
    private BoatAI currentBoatAi;
    private GameObject cannonBallHand;

    private HashSet<int> _cannonGroups = new HashSet<int>();

    public HashSet<int> cannonGroups
    {
        get => _cannonGroups;
        set
        {
            _cannonGroups = value;
            UnsubscribeCannon();
        }
    }
    [SerializeField]
    private CannonInterface nearestCannon;
    private CannonBallSetType nearestBallPile;

    private PandaBehaviour pandaBT = null;
    private Animator animator; 
    private ShipAmunitionInterface shipAmunitions;
    private BoatAI boatAi;
    private AIMovement movement;

    #endregion

    #region Mono Behavior
    void Start()
    {
        shipAmunitions = transform.parent.parent.GetComponent<ShipAmunitionInterface>();
        boatAi = transform.parent.parent.GetComponent<BoatAI>();
        movement = gameObject.GetComponent<AIMovement>();
        animator = gameObject.GetComponentInChildren<Animator>();
        pandaBT = gameObject.GetComponent<PandaBehaviour>();
        currentBoatAi = gameObject.GetComponentInParent<BoatAI>();
        currentAction = "Gunner";

    }

    #endregion

    #region setters and getters
    //*************setters and getters***************
    public void SetAction(string act) {
        UnsubscribeCannon();
        currentAction = act;
    }
    public void SetActionNoUn(string act) {
        currentAction = act;
    }

    public string GetAction() {
        return currentAction;
    }

    [Task]
    public bool GetBroadsideMode(string s) {
        if (s == "broadside" ? BroadsideMode : !BroadsideMode) {
            Task.current.Succeed();
        } else {
            Task.current.Fail();
        }
        return s == "broadside" ? BroadsideMode : !BroadsideMode;
    }

    #endregion

    #region Methods
    public void Die() {
        UnsubscribeCannon();
    }

    public void UnsubscribeCannon() {
        if (nearestCannon != null) {
            nearestCannon.StopWorkingOnCannon();
            //Debug.Log("Debug:Unit:" + name + ":unsubscribe cannon: " + nearestCannon.name);
        }
        nearestCannon = null;

    }

    private float PredictCannonAngle(float distance, float firePower)
    {
        const float gravity = 9.81f; // Standard gravity in m/s^2
    
        // Ensure that the inputs make sense and avoid edge cases
        if (distance <= 0f || firePower <= 0f)
        {
            return 45f; // Default angle if the inputs are invalid
        }

        // Calculate the angle using the projectile motion formula
        float term = gravity * distance / (firePower * firePower);
        if (term > 1f) // If this condition is met, it means the target is out of range
        {
            return 45f; // Default angle for unreachable targets
        }

        // Calculate the angle in radians and convert it to degrees
        float angleInRadians = Mathf.Asin(term) / 2f;
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

        return angleInDegrees;
    }

    #endregion

    #region Panda BT Scripts

    [Task]
    public void ActionCheck(string action) {
        Task.current.debugInfo = action;

        if (action == currentAction) {
            Task.current.Succeed();
        }
        else {
            Task.current.Fail();
        }
    }

    [Task]
    public void SetWanderPoint() {
        Vector3 wanderPoint = new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10));
        //Debug.Log("Info:Unit:Wander point" + wanderPoint);
        //movement.SetDestination(shipAmunitions.transform.position + wanderPoint);
        Task.current.Succeed();
    }
    [Task]
    public void Idle()
    {
        pandaBT.Wait(1f);
        return;
    }

    [Task]
    public void GoToClosest(string type)
    {
        Transform destination = null;
        Vector3 offset = new Vector3(0, 0, 0);
        if (type == "Fire" || type == "Reload" || type == "Rotate") {
            if (nearestCannon != null) {
                destination = nearestCannon.transform;
                offset = destination.forward * -4f;

            }
        }
        else if (type == "CannonBall") {
            if (hasCannonBall == true) {
                Task.current.Succeed();
                return;
            }
            if (nearestBallPile != null) {
                destination = nearestBallPile.transform;
            }
        }

        if (destination != null)
        {
            movement.SetDestination(destination.position + offset);
            Task.current.debugInfo = "" + Vector3.Distance(transform.position, destination.position + offset);

            if (Vector3.Distance(transform.position, destination.position + offset) < 1.3f)
            {
                Task.current.Succeed();
                return;
            }
        }
        else {
            Task.current.Fail();
        }
       
    }
    [Task]
    public void VariableWait(string input) {
        if (input == "FireCannon")
            pandaBT.Wait(1f);
        else if (input == "ReloadCannon") {
            pandaBT.Wait(5f);
        }else if (input == "RotateCannon") {
            pandaBT.Wait(1f);
        }
        else {
            pandaBT.Wait(1f);
        }
    }

    [Task]
    public bool HasAnyTargetableCannon(string input)
    {
        CannonInterface[] cannonList = null;

        if (input == "All") {
            cannonList = shipAmunitions.GetCannons();
        }
        else if (input == "Fire") {
            cannonList = shipAmunitions.GetLoadedCannons(cannonGroups);
        }
        else if (input == "Reload") {
            cannonList = shipAmunitions.GetUnloadedCannons();
        }

        return cannonList != null && cannonList.Length > 0;
    }


    [Task]
    public void FindClosestCannon(string input)
    {
        CannonInterface[] cannonList = null;
        //Debug.Log("Debug:Unit:Closest Cannon:" + name + " start search" + input);
        if (nearestCannon != null) {
            Task.current.Succeed();
            return;
        }
        UnsubscribeCannon();
        if (input == "All") {
            cannonList = shipAmunitions.GetCannons();
        }
        else if (input == "Fire") {
            cannonList = shipAmunitions.GetLoadedCannons(cannonGroups);
        }
        else if (input == "Reload") {
            cannonList = shipAmunitions.GetUnloadedCannons();
        }
        else if (input == "Rotate") {
            cannonList = shipAmunitions.GetRotateCannons();
        }
        


        float shortestDistance = 100f;
        float distance;
        foreach (CannonInterface cannon in cannonList) {
            if (!cannon.isBeingWorkedOn) {
                distance = Vector3.Distance(transform.position, cannon.transform.position);
                //Debug.Log("Debug:Unit:"+name + "Cannon: " + cannon.name + "Distance: " + distance);
                if (shortestDistance > distance) {
                    shortestDistance = distance;
                    nearestCannon = cannon;
                }
            }
        }
        if (nearestCannon != null ) {
            nearestCannon.isBeingWorkedOn = true;
            Task.current.debugInfo = nearestCannon.name;
            //Debug.Log("Debug:Unit:Closest Cannon:" + name + " end search" + nearestCannon.name);
            Task.current.Succeed();
        }
        else {
            //Debug.Log("failed to find closest");
            Task.current.Fail(); 
        }
    }

    

    [Task]
    public void FireCannon(string unsub)
    {
        nearestCannon.Fire();
        animator.SetTrigger("FireCannon");
        if (unsub == "True") {
            UnsubscribeCannon();
        }
        Task.current.Succeed();
    }

    [Task]
    public void GetCannonBall()
    {
        if (hasCannonBall != true) {
            hasCannonBall = true;
            cannonBallHand = Instantiate(cannonBallPrefab, gameObject.transform);
            cannonBallHand.transform.position = cannonBallHand.transform.position + new Vector3(1, 3, 0);
        }
        Task.current.Succeed();
    }
    [Task]
    public void FindClosestCannonBall() {
        
        CannonBallSetType[] cannonBallList = shipAmunitions.GetCannonBallPiles();
        
        float shortestDistance = 1000000000;
        float distance;

        foreach (CannonBallSetType cannonBall in cannonBallList) {
            distance = Vector3.Distance(transform.position, cannonBall.transform.position);
            //Debug.Log("Cannon: " + cannon.name + "Distance: " + distance);
            if (shortestDistance > distance) {
                shortestDistance = distance;
                nearestBallPile = cannonBall;
                Task.current.Succeed();
            }
        }
        
    }

    [Task]
    public void Reload()
    {
        if (nearestCannon == null) {
            Task.current.Fail();
            return;
        }

        if (hasCannonBall && !nearestCannon.GetLoadStatus()) {
            hasCannonBall = false;
            nearestCannon.LoadGun();
            Destroy(cannonBallHand);
            UnsubscribeCannon();
            Task.current.Succeed();
            return;
        }
        nearestCannon = null;
        Task.current.Fail();
    }
    
    [Task]
    public void RotateCannon()
    {
        if (Task.current.isStarting)
        {
            startTime = Time.time;
        }

        if (currentBoatAi.targetEnemy == null || nearestCannon == null || (Time.time - startTime > 10f))
        {
            //Debug.LogError("Target enemy or cannon is not assigned!");
            Task.current.Fail();
            return;
        }

        nearestCannon.WorkOnCannon();
        // Step 1: Predict the target's future position
        Vector3 targetPosition = currentBoatAi.targetEnemy.transform.position;
        float distance = Vector3.Distance(gameObject.transform.position, targetPosition);
        Vector3 targetDirection = new Vector3(currentBoatAi.targetEnemy.transform.forward.x, 0, currentBoatAi.targetEnemy.transform.forward.z);
        float targetSpeed = (currentBoatAi.targetEnemy.GetSpeed() * currentBoatAi.targetEnemy.GetEngineSpeed());
        float bulletTravelTime = distance / nearestCannon.fireForce;

        Vector3 predictionVec = targetPosition + targetDirection * targetSpeed * bulletTravelTime;
        Debug.DrawLine(predictionVec, predictionVec + new Vector3(0, 100, 0),Color.black);

        // Step 2: Calculate the direction and desired angles
        Vector3 directionToEnemy = predictionVec - nearestCannon.transform.position;
        float horizontalDistance = new Vector3(directionToEnemy.x, 0, directionToEnemy.z).magnitude;
        
        //calculate vertical
        float desiredVerticalAngle = PredictCannonAngle(horizontalDistance, nearestCannon.fireForce);
        desiredVerticalAngle -= Vector3.SignedAngle(nearestCannon.transform.up, Vector3.up, nearestCannon.transform.right); //adjust for ship tilt

        // Calc horizontal angle
        Vector3 localDirection = nearestCannon.transform.InverseTransformDirection(directionToEnemy.normalized);
        float desiredHorizontalAngle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;

        // Step 3: Set desired angles
        nearestCannon.WantedVerticalAngle = desiredVerticalAngle;
        nearestCannon.WantedHorizontalAngle = desiredHorizontalAngle;

        // Step 4: Check if the cannon is aligned
        bool isAligned = Mathf.Abs((-nearestCannon.currentVerticalAngle) - nearestCannon.WantedVerticalAngle) < .05f &&
                         Mathf.Abs(nearestCannon.currentHorizontalAngle - desiredHorizontalAngle) < .05f;
        Task.current.debugInfo = "is alligned:" + isAligned.ToString() +"time:" + (Time.time - startTime) +":"+ Mathf.Abs((-nearestCannon.currentVerticalAngle) - desiredVerticalAngle).ToString() + ":" + Mathf.Abs(nearestCannon.currentHorizontalAngle - desiredHorizontalAngle);
        if (isAligned) {
            Task.current.Succeed();
        }

    }

    [Task]
    public void AdjustSail()
    {
        Task.current.Succeed();
    }
    [Task]
    public void RepairShip()
    {
        Task.current.Succeed();
    }
    [Task]
    public void Repair()
    {
        Task.current.Succeed();
    }
    [Task]
    public void Steer()
    {
        Task.current.Succeed();
    }
    #endregion

}
