using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

public class PandaUnitAI : MonoBehaviour
{
    // Start is called before the first frame update
    #region variables

    public string currentAction;
   
    public float fireSpeed;
    private bool hasCannonBall = false;

    public GameObject cannonBallPrefab;
    private GameObject cannonBallHand;

    private HashSet<int> cannonGroups = new HashSet<int>();
    private CannonInterface nearestCannon; // need to set to null on death or action change. Also set the cannon to be not busy
    private CannonBallSetType nearestBallPile;

    private PandaBehaviour pandaBT = null;
    private Animator animator; 
    private ShipAmunitionInterface shipAmunitions;
    private AIMovement movement;

    #endregion

    #region Mono Behavior
    void Start()
    {
        shipAmunitions = transform.parent.parent.GetComponent<ShipAmunitionInterface>();
        movement = gameObject.GetComponent<AIMovement>();
        animator = gameObject.GetComponentInChildren<Animator>();
        pandaBT = gameObject.GetComponent<PandaBehaviour>();

        
    }

    #endregion

    #region setters and getters
    //*************setters and getters***************
    public void SetAction(string act) {
        UnsubscribeCannon();
        currentAction = act;
    }
    public string GetAction() {
        return currentAction;
    }
    public void SetCannonGroups(HashSet<int> groups) {
        cannonGroups = null;
        cannonGroups = new HashSet<int>(groups);
    }
    public HashSet<int> GetCannonGroups(){
        return cannonGroups;
    }

    
    #endregion

    #region Methods
    public void UnsubscribeCannon() {
        if (nearestCannon != null) {
            nearestCannon.SetBusyStatus(false);
            Debug.Log("Debug:Unit:" + name + ":unsubscribe cannon: " + nearestCannon.name);
        }
        nearestCannon = null;

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
        //Debug.Log("Info:Unit:Wander point " + wanderPoint);
        movement.SetDestination(shipAmunitions.transform.position + wanderPoint);
        Task.current.Succeed();
    }
    [Task]
    public void Idle()
    {
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
                offset = destination.forward * -3f;
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
        
        if (destination != null) {
            movement.SetDestination(destination.position + offset);
            Task.current.debugInfo = "" + Vector3.Distance(transform.position, destination.position + offset);
            if (Vector3.Distance(transform.position, destination.position +offset) < 1f) {
                Task.current.Succeed();
                return;
            }
        }
        //maybe implement timer ticking, reaches certain time then fail
        
    }
    [Task]
    public void VariableWait(string input) {
        if (input == "FireCannon")
            pandaBT.Wait(1f);
        else if (input == "ReloadCannon") {
            pandaBT.Wait(10f);
        }else if (input == "RotateCannon") {
            pandaBT.Wait(5f);
        }
        else {
            pandaBT.Wait(1f);
        }
    }

    [Task]
    public void FindClosestCannon(string input)
    {
        CannonInterface[] cannonList = null;

        if (nearestCannon != null) {
            Task.current.Succeed();
            return;
        }
        UnsubscribeCannon();

        //Debug.Log("Debug:Unit:Closest Cannon:" + name + " start search" + input);
        if (input == "Fire") {
            cannonList = shipAmunitions.GetLoadedCannons(cannonGroups);
            
        }
        else if (input == "Reload") {
            cannonList = shipAmunitions.GetUnloadedCannons();
        }
        else if (input == "Rotate") {
            cannonList = shipAmunitions.GetRotateCannons();
        }
        if (cannonList.Length < 1) {
            //Debug.Log("no Cannons to in list, Idle");
            SetAction("Idle");
        }


        float shortestDistance = 100f;
        float distance;
        foreach (CannonInterface cannon in cannonList) {
            if (!cannon.GetBusyStatus()) {
                distance = Vector3.Distance(transform.position, cannon.transform.position);
                Debug.Log("Debug:Unit:"+name + "Cannon: " + cannon.name + "Distance: " + distance);
                if (shortestDistance > distance) {
                    shortestDistance = distance;
                    nearestCannon = cannon;
                }
            }
        }
        if (nearestCannon != null ) {
            nearestCannon.SetBusyStatus(true);
            Task.current.debugInfo = nearestCannon.name;
            Debug.Log("Debug:Unit:Closest Cannon:" + name + " end search" + nearestCannon.name);
            Task.current.Succeed();
        }
        else {
            Task.current.Fail();
        }
    }

    [Task]
    public void FireCannon()
    {
        nearestCannon.Fire();
        animator.SetTrigger("FireCannon");
        UnsubscribeCannon();
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
        Task.current.Fail();
        CannonInterface[] cannonList = shipAmunitions.GetUnloadedCannons();
        if (cannonList.Length < 1) {
            //Debug.Log("no Cannons to reload");
            SetAction("Idle");
            Task.current.Fail();
            return;
        }
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
        
        if (hasCannonBall && !nearestCannon.GetLoadStatus()) {
            hasCannonBall = false;
            nearestCannon.LoadGun();
            Destroy(cannonBallHand);
            Task.current.Succeed();
            nearestCannon = null;
            return;
        }
        nearestCannon = null;
        Task.current.Fail();
    }
    [Task]
    public void RotateCannon() {
        nearestCannon.RotateBarrel();
        animator.SetTrigger("FireCannon");
        UnsubscribeCannon();
        Task.current.Succeed();
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
