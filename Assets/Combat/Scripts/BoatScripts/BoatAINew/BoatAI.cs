
using System.Collections.Generic;
using System.Linq;
using MapMode.Scripts.DataTypes.boatComponents.Cannons;
using UnityEngine;
using Panda;

//methods and functions for panda bt tree
public class BoatAI : MonoBehaviour
{
    #region variables

    [Header("Drive by Tuning")]
    [Tooltip("Meters to stay abeam from the target ship.")]
    [SerializeField] private float lateralOffset = 25f;
    [Tooltip("How far ahead of the enemy to aim (seconds of lead).")]
    [SerializeField] private float leadTime = 3.0f;
    [Tooltip("Add an extra 'exit' point beyond the enemy so you truly drive past.")]
    [SerializeField] private float overrunDistance = 40f;
    
    [SerializeField]
    private GameObject selectionIndicator;
    private BoatMaster boatMaster;
    public BoatControls boatControl;
    public BoatSteeringControls boatSteeringControl;
    private ShipCrewCommand shipCrewCommand;
    private ShipAmunitionInterface shipAmoInter;
    private BoatHealth boatHP;

    private int teamNumber;

    private (int x, int y) prevXYPos = (-1, -1);
    private (int x, int y) XYDest;

    private float runTime = 0;
    private float recalcTimer = float.MaxValue;

    private bool turningToTarget = false;//legacy TODO remove? idk

    private bool targetSetByCommander = false;
    
    public BoatAI targetEnemy
    {
        get => _targetEnemy; 
        set
        {
            if (_targetEnemy != value)
            {
                _targetEnemy = value;
                targetSetByCommander = true; 
            }
        }
    }
    [SerializeField]
    private BoatAI _targetEnemy;
    public Vector2 attackVector;
    public AttackSide attackDirection;

    private PandaBehaviour pandaBT = null;
    [Task]
    public string action;
    
    [Task]
    public bool isDead = false;
    [Task]
    public bool playerOnBoard = false;
    [Task]
    public int maxRange = 1000;

    #endregion

    #region mono behavior functions
    void Start()
    {
        boatMaster = gameObject.GetComponentInParent<BoatMaster>();
        boatControl = gameObject.GetComponent<BoatControls>();
        boatSteeringControl = gameObject.GetComponent<BoatSteeringControls>();
        shipCrewCommand = gameObject.GetComponent<ShipCrewCommand>();
        shipAmoInter = gameObject.GetComponent<ShipAmunitionInterface>();
        boatHP = gameObject.GetComponentInChildren<BoatHealth>();
        selectionIndicator.SetActive(false);
        maxRange = (int)boatControl.boat.GetMaxCannonRange();
    }
    

    #endregion
    
    #region setters and getters
    //*************setters and getters***************
    public int GetHP() {
        return boatHP.currentHealth;
    
    }
    public void SetIsDead(bool dead) {
        isDead = dead;
        if (dead == true) {
            boatMaster.DestroyBoat(this);
            Destroy(gameObject, 10);
        }
    }
    public bool GetIsDead() {
        return isDead;
    }

    public void SetDestination(float x, float z)
    {
        boatSteeringControl.SetTargetPosition(new Vector3(x, 10, z));
        return;
    }
    public (int x, int y) GetDestination() {
        return XYDest;
    }
    //previous xyPos on array
    public (int x, int y) GetPrevXYPos() {
        return prevXYPos;
    }
    

    //sets current action ship is taking
    public bool SetAction(string act) {
        action = act;
        return true;
    }

    public string GetAction() {
        return action;
    }
    
    //gets team number
    public int GetTeamNumber() {
        return teamNumber;
    }
    public void SetTeamNumber(int team) {
        teamNumber = team;
        Transform t = transform.Find("Colonial Ship");
        //Debug.Log("what is this" + t.name);
        AddLayerRecursively( t,"Team"+teamNumber);
        gameObject.tag = "Team" + teamNumber;
    }

    //returns current way boat is facing
    public Vector2 GetCardDirect() {
        Vector2 vec2D = new Vector2(transform.forward.x, transform.forward.z).normalized;
        return vec2D;
    }
    //returns current speed boat is going
    public float GetSpeed() {
        return boatControl.GetForward();
    }
    public float GetEngineSpeed() {
        return boatControl.GetEnginePower();
    }

    [Task]
    public bool IsAttacking()
    {
        if (action.Equals("FireAtWill") || action.Equals("DriveBy") || action.Equals("Ram") || action.Equals("ApproachTurnShoot"))
            return true;
        else{
            return false;
        }
    }

    #endregion

    #region Methods
    public void SetSelected(bool isSelected)
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(isSelected);
        }
    }

    void AddLayerRecursively(Transform trans, string layer)
    {
        //Debug.Log("obejct:"+ trans.name +"layer:"+layer+ ":layernum:"+ LayerMask.NameToLayer(layer));
        trans.gameObject.layer = LayerMask.NameToLayer(layer);
        if (trans.childCount > 0)
            foreach (Transform t in trans)
                AddLayerRecursively(t, layer);
    }
    
    #endregion

    #region Panda BT Scripts
    //*****Far away*****
    [Task]
    public void Idle()
    {
        return;
    }

    [Task]
    public void ChooseWanderDest() {
        BoatAI enemyBoat = boatMaster.GetClosestBoat(transform.position, teamNumber == 1 ? 2 : 1);
        if (enemyBoat != null) {
            SetDestination(enemyBoat.transform.position.x, enemyBoat.transform.position.z);
        }
        else {
            SetDestination(Random.Range(-100,100), Random.Range(-100,100));
        }
        //Debug.Log("wander: setting boat Destination in script 20 ,1");
        Task.current.Succeed();
    }
    

    
    //*****Run away*****
    [Task]
    public void ChooseRunAwayLocation()
    {
        // in future maybe have run to spawn... scan nearby areas for enemy ships ext maybe have it activly avaid enemys
        SetDestination(1000, 1400);

        Task.current.Succeed();
    }
    [Task]
    public void Reload() {
        shipCrewCommand.ReloadCannons();
        Task.current.Succeed();
    }

    //****** Close by *****
    [Task]
    public void ActionCheck(string s)
    {
        Task.current.debugInfo = action + "," + s + " Action t/f:" + action.Equals(s);
        if (action == s)
            Task.current.Succeed();
        else
            Task.current.Fail();
    }

    [Task]
    public void GetInAttackPosition(float distance)
    {
        Task.current.Succeed();
    }

    [Task] // TODO 12/5 expand into larger sub system with smart overall decisions for choosing who to attack. might be controlled by a seperate "commander" ai.
    public void ChooseEnemy() {

        if (_targetEnemy == null || _targetEnemy.gameObject == null)
        {
            targetSetByCommander = false;
            Debug.Log("Target enemy is null or destroyed. Resetting targetSetByCommander to false.");
        }
        if (targetSetByCommander) {
            Task.current.Succeed();
            return;
        }
        _targetEnemy = null;
        
        BoatAI[] boatAI = boatMaster.GetTeamBoats(teamNumber == 1 ? 2 : 1);
        float closestDistance = 9999999999999999999999f;
        foreach (BoatAI boat in boatAI) {
            if (boat.teamNumber != teamNumber && boat.isDead == false) {
                float distance = (boat.transform.position - (transform.position + transform.forward * 100)).sqrMagnitude;
                Debug.DrawLine(transform.position + transform.forward * 100, boat.transform.position, Color.red, 2f);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    _targetEnemy = boat;
                }

            }
        }
        
        if (_targetEnemy != null) {
            Vector2 boatDirect = GetCardDirect();
            Vector2 targetVec = new Vector2((_targetEnemy.transform.position - transform.position).x, (_targetEnemy.transform.position - transform.position).z);
            float dot = Vector2.Dot(boatDirect, targetVec) / targetVec.magnitude;

            Task.current.debugInfo = "enemy targeted: " + _targetEnemy.name + "distance" + Mathf.Sqrt(closestDistance) + "dot:" + dot;
            //Debug.Log(Task.current.debugInfo );


            bool hasCarronade = shipAmoInter.GetCannons().Any(c => c.cannonType == CannonType.Carronade);
            if (closestDistance > Mathf.Pow(3500, 2) && !hasCarronade)
            {
                Task.current.Fail();
            }
            
            else if (closestDistance > Mathf.Pow(200, 2) && !hasCarronade){
                SetAction("FireAtWill");
                shipCrewCommand.SetFireReloadAll(false);
            }else{
                SetAction("DriveBy");
                shipCrewCommand.SetFireReloadAll(false);
            }
            

            Task.current.Succeed();
        } else {
            Task.current.Fail();
        }
    }
    
    [Task]
    public void SetCannonsNeutral()
    {
        shipCrewCommand.SetCannonSets(3);
        shipCrewCommand.SetCannonAnglePredictions(0,0);
        shipCrewCommand.SetCannonSets(4);
        shipCrewCommand.SetCannonAnglePredictions(0, 0);
        //shipCrewCommand.AdjustCannonAngles();
        Task.current.Succeed();
    }

    [Task]
    public void UnitsToCannons()
    {
        HashSet<int> cannonGroups = new HashSet<int>();
        Task.current.debugInfo = "attack Direction:" + attackDirection;
        shipCrewCommand.ClearCannons();
        if (attackDirection == AttackSide.Left) {
            cannonGroups.Add(3);
            shipCrewCommand.SetCannonSets(3);
        }
        if (attackDirection == AttackSide.Right) {
            cannonGroups.Add(4);
            shipCrewCommand.SetCannonSets(4);
        }
        shipCrewCommand.PrepFireCannons();
        Task.current.Succeed();
    }

    [Task]
    public void DriveBeside()
    {
        runTime += Time.deltaTime;
        recalcTimer += Time.deltaTime;
        if (runTime > 5f) {
            runTime = 0;
            recalcTimer = float.MaxValue;
            Task.current.Fail();
            return;
        }

        if (_targetEnemy == null){
            runTime = 0;
            recalcTimer = float.MaxValue;
            Task.current.Fail();
            return;
        }

        if (recalcTimer >= 1){
            recalcTimer = 0;
            attackDirection = ChooseAttackDirection();
            var destination = FindDriveByTarget(attackDirection);
            boatSteeringControl.SetTargetPosition(destination);
            
        }

        
        if (boatSteeringControl.DistanceToTarget < 100) {
            Task.current.Succeed();
            runTime = 0;
            recalcTimer = float.MaxValue;
            return;
        }
        
    }
    
    /// <summary>
    /// Computes 1-2 waypoints on the chosen broadside. First is the "entry" point you should steer to now.
    /// Second (optional) is an "exit" point to ensure you pass and clear.
    /// </summary>
    public Vector3 FindDriveByTarget(AttackSide side)
    {
        if (_targetEnemy == null)
        {
            // Fallback: just go forward a bit
            return transform.position + transform.forward * 25f;
        }

        // --- 2D math on XZ plane ---
        Vector2 meXZ    = new Vector2(transform.position.x, transform.position.z);
        Vector2 enemyXZ = new Vector2(_targetEnemy.transform.position.x, _targetEnemy.transform.position.z);

        // Direction from ME → ENEMY (defines my right/left as I face the enemy)
        Vector2 approach  = (enemyXZ - meXZ).normalized;

        // Perpendiculars relative to *my* facing toward the enemy:
        // Right = (y, -x); Left = -Right
        Vector2 rightPerp = new Vector2(approach.y, -approach.x);
        Vector2 sidePerp  = (side == AttackSide.Right) ? rightPerp : -rightPerp;

        // Lead ahead of where the enemy will be
        Vector2 enemyFwd = new Vector2(_targetEnemy.transform.forward.x, _targetEnemy.transform.forward.z).normalized;
        float   enemySpd = Mathf.Max(0f, _targetEnemy.GetSpeed());
        float   leadDist = enemySpd * leadTime;

        // Entry point: ahead of enemy + lateral offset on chosen side
        Vector2 entryXZ = enemyXZ + enemyFwd * leadDist + sidePerp * lateralOffset;

        // Back to 3D
        Vector3 entry3D = new Vector3(entryXZ.x, 10, entryXZ.y);

        // Debug gizmos (optional)
        Debug.DrawLine(transform.position, _targetEnemy.transform.position, Color.yellow, 0.05f);
        Debug.DrawLine(_targetEnemy.transform.position,
            new Vector3(entry3D.x, 10, entry3D.z),
            Color.cyan, 0.05f);

        return entry3D;
    }


    [Task]
    public void CheckToFire()
    {
        int layerMask;
        Vector3 direction;
        runTime += Time.deltaTime;
        Task.current.debugInfo = "runTime:" + runTime;
        if (runTime > 5f) {
            runTime = 0;
            Task.current.Fail();
        }

        if (GetTeamNumber() == 1) {
            layerMask = 1 << 14;
        }
        else {
            layerMask = 1 << 13;
        }
        
        direction = transform.TransformDirection(Vector3.left);
        if (Physics.Raycast(transform.position + transform.forward * 10 + direction * 10, direction, out RaycastHit hit, 120f, layerMask)) {
            Debug.DrawRay(transform.position + transform.forward * 10 + direction * 10, direction * hit.distance, Color.red);
            Task.current.debugInfo = "Object hit: " + hit.collider +"layer: " + hit.collider.gameObject.layer +" " + GetTeamNumber();
            attackDirection = AttackSide.Left;
            runTime = 0;
            Task.current.Succeed();
        }
        else
            Debug.DrawRay(transform.position+ transform.forward * 10, direction * 100, Color.white);
        direction = transform.TransformDirection(Vector3.right);
        if (Physics.Raycast(transform.position +transform.forward*10+ direction * 10, direction, out hit, 120f, layerMask)) {
            Debug.DrawRay(transform.position + transform.forward * 10 + direction * 10, direction * hit.distance, Color.red);
            Task.current.debugInfo = "Object hit: " + hit.collider;
            attackDirection = AttackSide.Right;
            runTime = 0;
            Task.current.Succeed();
        }
        else
            Debug.DrawRay(transform.position + transform.forward * 10, direction * 100, Color.white);

    }
    [Task]
    public void TurnToFire()
    {
        var side = ChooseAttackDirection();
        boatSteeringControl.circle = true;
        boatSteeringControl.CircleClockwise = (side == AttackSide.Right);
        Task.current.Succeed();
    }


    [Task]
    public void Fire()
    {
        runTime += Time.deltaTime;
        Task.current.debugInfo += "runTime:" + runTime;
        if (runTime > 10f) {
            runTime = 0;
            Task.current.Fail();
        }
        HashSet<int> cannonGroups = new HashSet<int>();
        Task.current.debugInfo = "attack Direction:" + attackDirection;
        if (attackDirection == AttackSide.Left) {
            cannonGroups.Add(3);
            shipCrewCommand.SetCannonSets(3);
        }
        if (attackDirection == AttackSide.Right) {
            cannonGroups.Add(4);
            shipCrewCommand.SetCannonSets(4);
        }

        shipCrewCommand.FireCannons();
        if (shipAmoInter.GetLoadedCannons(cannonGroups).Length == 0) {
            Task.current.Succeed();
        }
    }

    [Task]
    public void RamSpeed() {
        boatControl.SetForward(1);
        int layerMask;
        BoatHealth enemyBoatHealth;
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        if (GetTeamNumber() == 1) {
            layerMask = 1 << 14;
        }
        else {
            layerMask = 1 << 13;
        }

        Debug.DrawRay(transform.position + transform.forward * 30, transform.forward * 3, Color.cyan);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 30, 3, layerMask);
        foreach (var hitCollider in hitColliders) {
            Debug.Log("RAM collider hit: " + hitCollider.gameObject + rigidBody.velocity.magnitude);
            enemyBoatHealth = hitCollider.GetComponentInParent<BoatHealth>();

            if (enemyBoatHealth != null && rigidBody.velocity.magnitude > 6) {
                Task.current.debugInfo = "Rammed enemy. Speed: " + rigidBody.velocity.magnitude;
                enemyBoatHealth.TakeDamage(20);
                Task.current.Succeed();
                SetAction("ApproachTurnShoot");
                break;
            }
        }

    }
    [Task]
    public void Die(){
        GetComponent<PandaBehaviour>().enabled = false;
        Task.current.Succeed();
    }

    [Task]
    public void FireAwayCommand() {
        HashSet<int> cannonGroups = new HashSet<int>();
        Task.current.debugInfo = "attack Direction:" + attackDirection;
        if (attackDirection == AttackSide.Left)
        {
            cannonGroups.Add(3);
            shipCrewCommand.SetCannonSets(3);
        }
        if (attackDirection == AttackSide.Right)
        {
            cannonGroups.Add(4);
            shipCrewCommand.SetCannonSets(4);
        }

        shipCrewCommand.FireAtWill();
        Task.current.Fail();
        
    }
    #endregion

    #region helper
    
    void ResetTree()
    {
        // Assuming the GameObject has a BehaviourTree component
        Panda.BehaviourTree bt = GetComponent<Panda.BehaviourTree>();
    
        if (bt != null)
        {
            bt.Reset(); // Resets the entire behavior tree
        }
    }
    
    public AttackSide ChooseAttackDirection()
    {
        Vector2 boatDirect = GetCardDirect();
        if (_targetEnemy == null) {
            return AttackSide.Right; // fallback default
        }

        Vector2 targetVec = new Vector2(
            _targetEnemy.transform.position.x - transform.position.x,
            _targetEnemy.transform.position.z - transform.position.z
        );
        targetVec += new Vector2(_targetEnemy.transform.forward.x, _targetEnemy.transform.forward.z) 
                     * 100 * (1 - Mathf.Pow(_targetEnemy.GetSpeed() - 1, 2));

        Vector2 targetVec90 = new Vector2(-targetVec.y, targetVec.x);
        float dotSideways = Vector2.Dot(boatDirect, targetVec90);

        return dotSideways > 0 ? AttackSide.Right : AttackSide.Left;
    }
    
    
    #endregion

}
