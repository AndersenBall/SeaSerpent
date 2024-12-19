using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Panda;

//methods and functions for panda bt tree
public class BoatAI : MonoBehaviour
{
    #region variables

    [SerializeField]
    private GameObject selectionIndicator;
    private BoatMaster boatMaster;
    private BoatControls boatControl;
    private ShipCrewCommand shipCrewCommand;
    private ShipAmunitionInterface shipAmoInter;
    private BoatHealth boatHP;

    private int teamNumber;

    private (int x, int y) prevXYPos = (-1, -1);
    private (int x, int y) XYDest;

    private float runTime = 0;

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
    public string attackDirection;

    private PandaBehaviour pandaBT = null;
    [Task]
    public string action;
    [Task]
    public bool attacking = true;
    [Task]
    public bool runningAway = false;
    [Task]
    public bool isDead = false;
    [Task]
    public bool playerOnBoard = false;


    #endregion

    #region mono behavior functions
    void Start()
    {
        boatMaster = gameObject.GetComponentInParent<BoatMaster>();
        boatControl = gameObject.GetComponent<BoatControls>();
        shipCrewCommand = gameObject.GetComponent<ShipCrewCommand>();
        shipAmoInter = gameObject.GetComponent<ShipAmunitionInterface>();
        boatHP = gameObject.GetComponentInChildren<BoatHealth>();
        selectionIndicator.SetActive(false);
    }
    

    #endregion


    #region setters and getters
    //*************setters and getters***************
    //set the location that the boat should travel to.
    public int GetHP() {
        return (int) boatHP.GetHealth();
    
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
    public void SetDestination((int x, int y) XY) {
        XYDest = XY;
        return;
    }
    public void SetDestination(int x, int y)
    {
        XYDest = (x, y);
        return;
    }
    public (int x, int y) GetDestination() {
        return XYDest;
    }
    //previous xyPos on array
    public (int x, int y) GetPrevXYPos() {
        return prevXYPos;
    }

    public void SetPrevXYPos((int x, int y) prev) {
        prevXYPos = prev;
    }

    //sets current action ship is taking
    public bool SetAction(string act) {
        action = act;
        return true;
    }

    public string GetAction() {
        return action;
    }
    //Set side boat hopes to shoot from
    public bool SetAttackDirection(string act)
    {
        attackDirection = act;
        return true;
    }

    public string GetAttackDirection()
    {
        return attackDirection;
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
    public void AllignToDestination()
    {
        Debug.Log("allign to destination called");
        if (turningToTarget == false) {
            //Debug.Log("Dot: " + Vector2.Dot(boatDirect, targetVec90) + " boat direct:" + boatDirect + " targetVec:" + targetVec90);
            StartCoroutine(TurnToDest());
        }
    }

    IEnumerator TurnToDest()
    {
        Debug.Log("boat:" + gameObject.name + " started alligning to:" + XYDest.x + "," + XYDest.y);
        turningToTarget = true;
        Vector2 boatDirect = GetCardDirect();
        Vector2 targetVec90 = new Vector2(XYDest.y - prevXYPos.y, -(XYDest.x - prevXYPos.x));
        float dotSideways = Vector2.Dot(boatDirect, targetVec90);
        while (dotSideways > .01f || dotSideways < -.01f) {
            //Debug.Log("Dot: " + dotSideways + " boat direct:" + boatDirect + " targetVec:" + targetVec90);
            if (dotSideways > .01f) {
                boatControl.SetTurn(-1);
            }
            else if (dotSideways < -.1f) {
                boatControl.SetTurn(1);
            }
            yield return new WaitForSeconds(.05f);
            boatDirect = GetCardDirect();
            targetVec90 = new Vector2(XYDest.y - prevXYPos.y, -(XYDest.x - prevXYPos.x));
            dotSideways = Vector2.Dot(boatDirect, targetVec90);
        }
        boatControl.SetTurn(0);
        turningToTarget = false;
        Debug.Log("alligned with destination");
    }


    //*************** Actions*****************


    #endregion

    #region Panda BT Scripts
    //************** Panda BT Script **********
    
    //*****Far away*****
    
    [Task]
    public void ChooseWanderDest() {
        BoatAI enemyBoat = boatMaster.GetClosestBoat(transform.position, teamNumber == 1 ? 2 : 1);
        if (enemyBoat != null) {
            SetDestination(enemyBoat.GetPrevXYPos());
        }
        else {
            SetDestination(GetPrevXYPos());
        }
        //Debug.Log("wander: setting boat Destination in script 20 ,1");
        Task.current.Succeed();
    }

    [Task]
    public void AllignToWanderPoint() {

        boatControl.SetForward(.5f);
        Vector2 boatDirect = GetCardDirect();
        Vector2 targetVec90 = new Vector2(XYDest.y - prevXYPos.y, -(XYDest.x - prevXYPos.x));
        float dotSideways = Vector2.Dot(boatDirect, targetVec90);
        float dotForward = Vector2.Dot(boatDirect, new Vector2(XYDest.x - prevXYPos.x, XYDest.y - prevXYPos.y));
        if (dotSideways <= .1f && dotSideways >= -.1f) {
            if (dotForward < 0) {
                boatControl.SetTurn(1);
            }
            else {
                boatControl.SetForward(1);
                boatControl.SetTurn(0);
                Task.current.Succeed();
            }
        }

        if (dotSideways > .1f) {
            boatControl.SetTurn(-1);
        }
        else if (dotSideways < -.1f) {
            boatControl.SetTurn(1);
        }

    }

    [Task]
    public void ScanForShips()
    {
        List<BoatAI> boatAI = boatMaster.NearbyBoats(this, 30);

        foreach (BoatAI b in boatAI) {
            Task.current.debugInfo = "boat:" + b.name;
            if (b.GetTeamNumber() != this.GetTeamNumber() && b.isDead == false) {
                attacking = true;
                Task.current.Fail();
                return;
            }
        }
        boatAI.Clear();
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
    [Task]
    public void CheckReload()
    {
        CannonInterface[] cannons = shipAmoInter.GetUnloadedCannons();
        Task.current.debugInfo = "cannons not loaded:" + cannons.Length;
        if (cannons.Length == 0) {
            attacking = false;
            runningAway = false;
        }
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
            SetAction("FireAtWill");
            //if ((transform.position - _targetEnemy.transform.position).sqrMagnitude > Mathf.Pow(3500, 2)) {
            //    Task.current.Fail();
            //    attacking = false;
            //}else if ((transform.position - _targetEnemy.transform.position).sqrMagnitude > Mathf.Pow(500, 2)) {
            //    SetAction("PotShot");
            //}else if ((transform.position - _targetEnemy.transform.position).sqrMagnitude < Mathf.Pow(200, 2) && dot < 0)
            //    SetAction("ApproachTurnShoot");
            //else
            //    SetAction("DriveBy");

            Task.current.Succeed();
        } else {
            attacking = false;
            Task.current.Fail();
        }
    }
    
    [Task]
    public void ChooseAttackDirection()
    {
        Vector2 boatDirect = GetCardDirect();
        if (_targetEnemy == null) {
            Task.current.Fail();
            return;
        }
        Vector2 targetVec = new Vector2(_targetEnemy.transform.position.x - transform.position.x, _targetEnemy.transform.position.z - transform.position.z);
        targetVec += new Vector2(_targetEnemy.transform.forward.x, _targetEnemy.transform.forward.z) * 100 * (1 - Mathf.Pow(_targetEnemy.GetSpeed() - 1, 2));

        Debug.DrawRay(transform.position, new Vector3(targetVec.x, 0, targetVec.y), Color.white, 1f);
        Vector2 targetVec90 = new Vector2(-targetVec.y, targetVec.x);

        float dotSideways = Vector2.Dot(boatDirect, targetVec90);
        if (dotSideways > 0) {
            SetAttackDirection("Right");
            Task.current.debugInfo = "Right";
        }
        else {
            SetAttackDirection("Left");
            Task.current.debugInfo = "Left";
        }
        Task.current.debugInfo = "enemy speed " + _targetEnemy.GetSpeed();
        Task.current.Succeed();
    }
    [Task]
    public void CreateAttackVector() {
        if (action == "DriveBy") {
            if (attackDirection == "Left")
                attackVector = new Vector2(_targetEnemy.GetPrevXYPos().y - prevXYPos.y, -(_targetEnemy.GetPrevXYPos().x - prevXYPos.x)).normalized * 2;
            else
                attackVector = new Vector2(_targetEnemy.GetPrevXYPos().y - prevXYPos.y, -(_targetEnemy.GetPrevXYPos().x - prevXYPos.x)).normalized * -2;
            Task.current.debugInfo = "attack vector: " + attackVector.x + "," + attackVector.y + " action: " + action;
            Task.current.Succeed();
        }
        else if (action == "FireAtWill" || action == "ApproachTurnShoot" || action == "Ram" || action == "PotShot") {
            attackVector = new Vector2(0, 0);

            Task.current.debugInfo = "attack vector: " + attackVector.x + "," + attackVector.y + " action: " + action;
            Task.current.Succeed();
        }
        else {
            Task.current.Fail();
        }
    }
    [Task]
    public void SetUpCannons()
    {
        if (attackDirection == "Right") {
            shipCrewCommand.SetCannonSets(4);
        }
        else {
            shipCrewCommand.SetCannonSets(3);
        }
        if (_targetEnemy == null) {
            
            Task.current.Fail();
            return;
        }
        float distance = Vector3.Distance(_targetEnemy.transform.position, transform.position);
        Vector3 targetVec = new Vector3(_targetEnemy.transform.forward.x,0 ,_targetEnemy.transform.forward.z) * distance/20 * (1 - Mathf.Pow(_targetEnemy.GetSpeed() - 1, 2));
        Debug.DrawLine(_targetEnemy.transform.position + targetVec, _targetEnemy.transform.position + targetVec + new Vector3(0, 100, 0));
        distance = Vector3.Distance(_targetEnemy.transform.position + targetVec, transform.position);
        shipCrewCommand.SetCannonAnglePredictions(Mathf.RoundToInt(PredictCannonAngle(distance)*2)/2f);
        shipCrewCommand.AdjustCannonAngles();//could be here
        Task.current.debugInfo = "cannons left to update: "+shipAmoInter.GetRotateCannons().Length + " wanted angle: " + Mathf.RoundToInt(PredictCannonAngle(distance));
        if (shipAmoInter.GetRotateCannons().Length == 0)
            Task.current.Succeed();
    }

    /*
    [Task]
    public void ResetCannons() {
        shipCrewCommand.SetCannonSets(3);
        shipCrewCommand.SetCannonAnglePredictions(0);
        shipCrewCommand.SetCannonSets(4);
        shipCrewCommand.SetCannonAnglePredictions(0);
        shipCrewCommand.AdjustCannonAngles();
    }
    */

    [Task]
    public bool GetInAttackPosition(float exitDistance)
    {
        runTime += Time.deltaTime;
        if (runTime > 10f) {
            runTime = 0;
            Task.current.Fail();
            return false;
        }

        float addedX = attackVector.x * boatMaster.tileSize;
        float addedY = attackVector.y * boatMaster.tileSize;
        if (_targetEnemy == null) {
            Task.current.Fail();
            return false;
        }
        (float x, float y) destination = (_targetEnemy.transform.position.x + addedX, _targetEnemy.transform.position.z + addedY);
        Debug.DrawRay(transform.position, _targetEnemy.transform.position + new Vector3(addedX, 0, addedY) - transform.position, Color.yellow);

        if (Mathf.Pow(destination.x - transform.position.x, 2) + Mathf.Pow(destination.y - transform.position.z, 2) < Mathf.Pow(exitDistance, 2)) {
            boatControl.SetTurn(0);
            runTime = 0;
            Task.current.Succeed();
            return true;
        }
        else {
            SetDestination(boatMaster.XYCordinates(destination.x, destination.y));

            AllignToVector(new Vector2(destination.x - transform.position.x, destination.y - transform.position.z));
            Task.current.debugInfo = Task.current.debugInfo + " RunTime:" + runTime;
            return false;
        }
        
    }
    [Task]
    public void CheckToFire()
    {
        int layerMask;
        Vector3 direction;
        runTime += Time.deltaTime;
        Task.current.debugInfo = "runTime:" + runTime;
        if (runTime > 8f) {
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
        if (Physics.Raycast(transform.position + transform.forward * 15 + direction * 10, direction, out RaycastHit hit, 120f, layerMask)) {
            Debug.DrawRay(transform.position + transform.forward * 15 + direction * 10, direction * hit.distance, Color.red);
            Task.current.debugInfo = "Object hit: " + hit.collider +"layer: " + hit.collider.gameObject.layer +" " + GetTeamNumber();
            SetAttackDirection("Left");
            runTime = 0;
            Task.current.Succeed();
        }
        else
            Debug.DrawRay(transform.position+ transform.forward * 15, direction * 100, Color.white);
        direction = transform.TransformDirection(Vector3.right);
        if (Physics.Raycast(transform.position +transform.forward*15+ direction * 10, direction, out hit, 120f, layerMask)) {
            Debug.DrawRay(transform.position + transform.forward * 15 + direction * 10, direction * hit.distance, Color.red);
            Task.current.debugInfo = "Object hit: " + hit.collider;
            SetAttackDirection("Right");
            runTime = 0;
            Task.current.Succeed();
        }
        else
            Debug.DrawRay(transform.position + transform.forward * 15, direction * 100, Color.white);

    }
    [Task]
    public void TurnToFire() {
        Vector2 boatDirect = GetCardDirect();

        Vector2 targetVec = new Vector2(_targetEnemy.transform.position.x - this.transform.position.x, _targetEnemy.transform.position.z - this.transform.position.z);
        Vector2 targetVec90 = new Vector2(-targetVec.y, targetVec.x);
        float dotForward = Vector2.Dot(boatDirect, targetVec);
        float dot90 = Vector2.Dot(boatDirect, targetVec90);
        Task.current.debugInfo = "dot: " + dotForward;
        if (dot90 < 0) {
            SetAttackDirection("Left");
            if (dotForward <= 10f && dotForward >= -10f) {
                boatControl.SetTurn(0);
                boatControl.SetForward(1f);
                Task.current.Succeed();
            }

            if (dotForward > 10f) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(1);
            }
            else if (dotForward < -10f) {
                boatControl.SetTurn(-1);
                boatControl.SetForward(.5f);
            }
        }
        else {
            SetAttackDirection("Right");
            if (dotForward <= 10f && dotForward >= -10f) {
                boatControl.SetTurn(0);
                boatControl.SetForward(1f);
                Task.current.Succeed();
            }

            if (dotForward > 10f) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(-1);
            }
            else if (dotForward < -10f) {
                boatControl.SetTurn(1);
                boatControl.SetForward(.5f);
            }
        }
    }

    [Task]
    public void TurnToFireFar()
    {
        int outer = 60;
        int inner = 8;
        if (_targetEnemy == null) {
            Task.current.Fail();
            return;
        }
        Vector2 boatDirect = GetCardDirect();
        float distance = Vector3.Distance(gameObject.transform.position, _targetEnemy.transform.position);
        Vector3 predictionVec = _targetEnemy.transform.position + new Vector3(_targetEnemy.transform.forward.x, 0, _targetEnemy.transform.forward.z) * distance / 15 * (1 - Mathf.Pow(_targetEnemy.GetSpeed() - 1, 2));
        Debug.DrawLine(predictionVec, predictionVec + new Vector3(0, 100, 0), Color.red);
        
        Vector2 targetVec = new Vector2(predictionVec.x - this.transform.position.x, predictionVec.z - this.transform.position.z).normalized * 100;
        float dotForward = Vector2.Dot(boatDirect, targetVec);
        
        Task.current.debugInfo = "dot: " + dotForward;
        if (attackDirection == "left") {
            SetAttackDirection("Left");
            if (dotForward >= -inner && dotForward <= inner) {
                boatControl.SetForward(.25f);
                boatControl.SetTurn(0);
                Task.current.Succeed();
            }else if (-outer <= dotForward && dotForward < -inner) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(-.5f);
            }
            else if (dotForward < -outer) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(-1f);
            }
            else if (inner < dotForward && dotForward <= outer) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(.5f);
            }
            else if (dotForward > outer) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(1f);
            }
        }
        else {
            SetAttackDirection("Right");
            if (dotForward >= -inner && dotForward <= inner) {
                boatControl.SetForward(.25f);
                boatControl.SetTurn(0);
                Task.current.Succeed();
            }
            else if (-outer <= dotForward && dotForward < -inner) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(.5f);
            }
            else if (dotForward < -outer) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(1f);
            }
            else if (inner < dotForward && dotForward <= outer) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(-.5f);
            }
            else if (dotForward > outer) {
                boatControl.SetForward(.5f);
                boatControl.SetTurn(-1f);
            }
            
        }
    }

    [Task]
    public void KeepAim()//more percise move slower
    {
        float min = .0001f;
        float upperLimit = .15f;
        Vector2 boatDirect = GetCardDirect();
        if (_targetEnemy == null) {
            Task.current.Fail();
            return;
        }
        float distance = Vector3.Distance(gameObject.transform.position,_targetEnemy.transform.position);
        Vector3 predictionVec = _targetEnemy.transform.position +new Vector3(_targetEnemy.transform.forward.x, 0, _targetEnemy.transform.forward.z)  *distance/15 * (1 - (Mathf.Pow(_targetEnemy.GetSpeed()-1, 2)* _targetEnemy.GetEngineSpeed() / 8));
        Debug.DrawLine(predictionVec, predictionVec + new Vector3(0, 100, 0),Color.green);
        Vector2 targetVec = new Vector2(predictionVec.x - this.transform.position.x, predictionVec.z - this.transform.position.z);
        Vector2 targetVec90 = new Vector2(-targetVec.y, targetVec.x);
        float dotForward = -3*Vector2.Dot(boatDirect, targetVec)/distance;
        float dot90 = Vector2.Dot(boatDirect, targetVec90);
        Task.current.debugInfo = "dot: " + dotForward;
        if (dot90 < 0) {
            SetAttackDirection("Left");
            if(dotForward >= -min && dotForward <= min) {
                boatControl.SetTurn(0);
                
            }

            if (dotForward > min) {
                boatControl.SetForward(.25f);
                boatControl.SetTurn(dotForward);
                if (dotForward > upperLimit) {
                    boatControl.SetForward(.5f);
                    boatControl.SetTurn(dotForward);
                }
            }
            else if (dotForward < -min) {
                boatControl.SetForward(.25f);
                boatControl.SetTurn(dotForward);
                if (dotForward < upperLimit) {
                    boatControl.SetForward(.5f);
                    boatControl.SetTurn(dotForward);
                }
            }
        }
        else {
            SetAttackDirection("Right");
            if (dotForward >= -min && dotForward <= min ) {
                boatControl.SetTurn(0);
        
            }

            if (dotForward > min) {
                boatControl.SetForward(.25f);
                boatControl.SetTurn(dotForward);
                if (dotForward > upperLimit) {
                    boatControl.SetForward(.5f);
                    boatControl.SetTurn(dotForward);
                }
            }
            else if (dotForward < -min) {
                boatControl.SetForward(.25f);
                boatControl.SetTurn(dotForward);
                if (dotForward < -upperLimit) {
                    boatControl.SetForward(.5f);
                    boatControl.SetTurn(dotForward);
                }
            }
        }
    }

    [Task]
    public void Fire()
    {
        HashSet<int> cannonGroups = new HashSet<int>();
        Task.current.debugInfo = "attack Direction:" + attackDirection;
        if (attackDirection == "Left") {
            cannonGroups.Add(3);
            shipCrewCommand.SetCannonSets(3);
        }
        if (attackDirection == "Right") {
            cannonGroups.Add(4);
            shipCrewCommand.SetCannonSets(4);
        }

        shipCrewCommand.FireCannons();
        if (shipAmoInter.GetLoadedCannons(cannonGroups).Length == 0) {
            Task.current.Succeed();
        }
    }
    [Task]
    public void ChooseToAttack() {
        attacking = true;
        Task.current.Succeed();
    
    }
    [Task]
    public void CheckToRetreat()
    {
        runningAway = true;
        attacking = false;
        Task.current.Succeed();
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
    public void Idle() {
        Task.current.Succeed();
    }
    [Task]
    public void FireAwayCommand() {
        HashSet<int> cannonGroups = new HashSet<int>();
        Task.current.debugInfo = "attack Direction:" + attackDirection;
        if (attackDirection == "Left")
        {
            cannonGroups.Add(3);
            shipCrewCommand.SetCannonSets(3);
        }
        if (attackDirection == "Right")
        {
            cannonGroups.Add(4);
            shipCrewCommand.SetCannonSets(4);
        }

        shipCrewCommand.FireAtWill();
        Task.current.Fail();
        
    }
    //***********helper methods***************
    public float PredictCannonAngle(float distance)
    {
        float value = Mathf.Asin((distance-7) / 4077.47f) / 2 * (180 / Mathf.PI);
        if (float.IsNaN(value)) {
            return 45f;
        }
        else {
            return value;
        }
    }
    private void AllignToVector(Vector2 targetVec) {
        Vector2 boatDirect = GetCardDirect();
        Vector2 targetVec90 = new Vector2(targetVec.y,-targetVec.x);
        string debugInf = "";

        float dotSideways = Vector2.Dot(boatDirect, targetVec90);
       
        if (dotSideways <= .75f && dotSideways >= -.75f) {
            boatControl.SetForward(1);
            boatControl.SetTurn(0);
            
        }
        else if (dotSideways > .75f) {
            if (dotSideways < 2f) {
                boatControl.SetForward(1f);
                boatControl.SetTurn(-.5f);
                debugInf = " Turn Left Little ";

            }
            else {
                boatControl.SetForward(.75f);
                boatControl.SetTurn(-1);
                debugInf = " Turn Left Lot ";
            }
        }
        else if (dotSideways < -.75f) {
            if (dotSideways > -2f) {
                boatControl.SetForward(1f);
                boatControl.SetTurn(.5f);
                debugInf = " Turn Right Little ";
            }
            else {
                boatControl.SetForward(.75f);
                boatControl.SetTurn(1);
                debugInf = " Turn Right Lot ";
            }
        }
        Task.current.debugInfo =  " my location:" + (int)GetPrevXYPos().x + "," + (int)GetPrevXYPos().y  +
                    _targetEnemy.GetPrevXYPos().x + "," + _targetEnemy.GetPrevXYPos().y + " Dot: " + (int)dotSideways + debugInf;
    }
    #endregion

}
