using System;
using System.Collections;
using System.Collections.Generic;
using MapMode.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class FleetMapController : MonoBehaviour
{
    #region variables
    [System.Serializable]
    public struct FleetData
    {
        public Fleet fleet;
        public float[] pos;
        public string destName;
    }
    public NavMeshAgent navAgent;
    [TextArea]
    private string pathStatusMessage = "";
    public Transform _destination;
    private Vector3 lastDestinationPosition;
    private float chaseUpdateInterval = 0.5f; 
    private float timeSinceLastUpdate = 0f;
    public Transform destination { get => _destination; set => _destination = value; }
    public FleetAIState CurrentState { get; private set; } = FleetAIState.Idle;
    public FleetMapController CurrentTarget { get; private set; }
    [SerializeField] private float aiTickInterval = 0.5f;
    [SerializeField] private float detectRadius = 200f;
    [SerializeField] private float fleePowerRatioThreshold = 1.1f;
    [SerializeField] private float regroupHpThreshold = 0.35f;
    private float aiTickTimer = 0f;
    private IFleetAIPolicy aiPolicy = new DefaultFleetAIPolicy();


    Fleet fleet;
    [TextArea]
    public string boatNames = "";
    #endregion
    #region Monobehabiours

    private void Awake()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        
    }
    void Start()
    {
        gameObject.name = "fleet" + fleet.FleetID;
        GameEvents.SaveInitiated += SaveNPCFleet;
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        (float fleetSpeed, float fleetAcceleration) = fleet.CalculateSpeed();
        navAgent.speed = fleetSpeed;
        navAgent.acceleration = fleetAcceleration;
        UpdateBoatNames();
        InitializeAIState();
    }

    private void Update()
    {

        if (navAgent != null)
        {
            aiTickTimer += Time.deltaTime;
            if (aiTickTimer >= aiTickInterval)
            {
                aiTickTimer = 0f;
                TickAI();
            }

            if (destination != null && destination.position != lastDestinationPosition)
            {
                timeSinceLastUpdate += Time.deltaTime;

                if (timeSinceLastUpdate >= chaseUpdateInterval)
                {
                    navAgent.SetDestination(destination.position);
                    lastDestinationPosition = destination.position;
                    timeSinceLastUpdate = 0f;
                }
            }

            // Check path status and update the message
            if (navAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                pathStatusMessage = "Path is valid.";
            }
            else if (navAgent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                pathStatusMessage = "Path is partially reachable.";
            }
            else if (navAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                pathStatusMessage = "Path is invalid.";
            }
        }
        else {
            Debug.LogError("FleetMapController needs a navagent");
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        Town town = other.GetComponent<Town>();
        FleetMapController otherFleet = other.GetComponent<FleetMapController>();

        if (otherFleet != null)
        {
            ResolveFleetEncounter(otherFleet);
        }
        else if (town != null && destination != null && town.name == destination.name)
        {
            if (fleet.FleetType == AIFleetType.Trade)
            {
                town.SellItemsInCargo(fleet, 10000, "All");
            }

            UpdateBoatNames();
            DockFleet(town);
        }
    }


    private void ResolveFleetEncounter(FleetMapController otherFleetController)
    {
        Fleet otherFleet = otherFleetController.GetFleet();
        if (fleet == null || otherFleet == null)
        {
            return;
        }

        if (!CanEngage(otherFleet))
        {
            return;
        }

        if (BattlePredicter.GetFleetPower(fleet) >= BattlePredicter.GetFleetPower(otherFleet))
        {
            BattlePredicter.ApplyBattleDamage(fleet, otherFleet);
            UpdateAfterEncounter(otherFleetController, wonBattle: true);
        }
        else
        {
            BattlePredicter.ApplyBattleDamage(otherFleet, fleet);
            UpdateAfterEncounter(otherFleetController, wonBattle: false);
        }
    }

    public void UpdateAfterEncounter(FleetMapController otherFleetController, bool wonBattle)
    {
        if (fleet == null)
        {
            return;
        }

        if (fleet.getNumberBoats() <= 0)
        {
            RemoveFleet();
            return;
        }

        aiPolicy.UpdateAfterEncounter(this, otherFleetController, wonBattle);
    }

    private FleetMapController FindClosestFleet(System.Func<Fleet, bool> predicate)
    {
        FleetMapController[] allFleets = FindObjectsOfType<FleetMapController>();
        FleetMapController closestFleet = null;
        float closestDistance = float.MaxValue;
        float maxDistanceSqr = detectRadius * detectRadius;

        foreach (FleetMapController fleetController in allFleets)
        {
            if (fleetController == this)
            {
                continue;
            }

            Fleet candidate = fleetController.GetFleet();
            if (candidate == null || !predicate(candidate))
            {
                continue;
            }

            float distance = (fleetController.transform.position - transform.position).sqrMagnitude;
            if (distance > maxDistanceSqr)
            {
                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestFleet = fleetController;
            }
        }

        return closestFleet;
    }

    void OnDestroy()
    {
        // Perform cleanup when the object is destroyed
        GameEvents.SaveInitiated -= SaveNPCFleet;
        if (fleet != null)
        {
            BoatAILead.RemoveID(fleet.FleetID);
            
        }
        else {
            Debug.Log("why is this null?");
        }
    }
    #endregion
    #region methods
    public void SetFleet(Fleet f) {fleet = f;}
    public Fleet GetFleet(){ return fleet;}
    public void ChangeState(FleetAIState newState) { CurrentState = newState; }
    public float RegroupHpThreshold => regroupHpThreshold;

    public void DockFleet(Town town) {
        town.DockFleet(fleet);
        RemoveFleet();
    }

    public void RemoveFleet()
    {
        string key = "boat" + fleet.FleetID;
        SaveLoad.DeleteSave(key);
        Destroy(gameObject, .1f);
    }

    #endregion
    #region DevMethods
    public void SaveNPCFleet() {
        FleetData data = new FleetData();
        data.fleet = fleet;
        Vector3 location = gameObject.transform.position;
        data.pos = new float[] { location.x, location.y, location.z };
        Debug.Log("save fleet:" + name + ":" + data.pos[0] + "," + data.pos[1] + "," + data.pos[2]);
        data.destName = destination != null ? destination.name : string.Empty;
        SaveLoad.Save<FleetData>(data, "boat"+fleet.FleetID);
    }
    public void LoadNPCFleet(int id) {
        FleetData data = SaveLoad.Load<FleetData>("boat"+id);
        fleet = data.fleet;
        gameObject.name = "fleet" + fleet.FleetID;
        if (fleet.GetBoats() == null || fleet.GetBoats().Count == 0)
        {
            RemoveFleet();
            return;
        }

        transform.position = new Vector3(data.pos[0], data.pos[1], data.pos[2]);
        Vector3 targetPosition = new Vector3(data.pos[0], data.pos[1], data.pos[2]);
        navAgent.Warp(targetPosition);

        Debug.Log("load fleet:" + name + ":" + data.pos[0] + "," + data.pos[1] + "," + data.pos[2] + "actual:" +transform.position);
        GameObject dest = GameObject.Find("/enviroment/towns/" +data.destName);
 
        if (dest != null) {
            destination = dest.transform; 
        }
        else {
            destination = GameObject.Find("/enviroment/towns/Havana").transform;
            Debug.Log("loaded fleet cant find destination:" + data.destName);
        }
    }
    public void UpdateBoatNames() {
        
        boatNames = fleet.ToString();
    }

    private void InitializeAIState()
    {
        CurrentTarget = null;
        aiPolicy = FleetAIPolicyFactory.CreateFor(fleet.FleetType);
        ChangeState(aiPolicy.GetInitialState(this));
    }

    private void TickAI()
    {
        if (fleet == null)
        {
            ChangeState(FleetAIState.Disabled);
            return;
        }

        if (fleet.getNumberBoats() <= 0)
        {
            ChangeState(FleetAIState.Disabled);
            RemoveFleet();
            return;
        }

        aiPolicy.Tick(this);
    }

    public void SetCurrentTarget(FleetMapController target)
    {
        CurrentTarget = target;
        if (target != null)
        {
            destination = target.transform;
        }
    }

    public void ClearCurrentTarget()
    {
        CurrentTarget = null;
    }

    public bool HasValidCurrentTarget()
    {
        return CurrentTarget != null && CurrentTarget.GetFleet() != null && CurrentTarget.GetFleet().getNumberBoats() > 0;
    }

    public void SyncDestinationToCurrentTarget()
    {
        if (CurrentTarget != null)
        {
            destination = CurrentTarget.transform;
        }
    }

    public bool ShouldFleeFrom(FleetMapController threat)
    {
        if (threat == null || threat.GetFleet() == null || fleet == null)
        {
            return false;
        }

        float selfPower = BattlePredicter.GetFleetPower(fleet);
        float threatPower = BattlePredicter.GetFleetPower(threat.GetFleet());
        return selfPower > 0f && (threatPower / selfPower) >= fleePowerRatioThreshold;
    }

    public void SetDestinationToNearestFriendlyTown()
    {
        Town safeTown = FindNearestFriendlyTown();
        if (safeTown != null)
        {
            destination = safeTown.transform;
        }
    }

    public FleetMapController FindNearestHostileFleet()
    {
        return FindClosestFleet(CanEngage);
    }

    private bool CanEngage(Fleet other)
    {
        return aiPolicy.CanEngage(this, other);
    }

    private Town FindNearestFriendlyTown()
    {
        Town[] allTowns = FindObjectsOfType<Town>();
        Town closestTown = null;
        float closestDistance = float.MaxValue;

        foreach (Town town in allTowns)
        {
            if (town.nationality != fleet.Nationality)
            {
                continue;
            }

            float distance = (town.transform.position - transform.position).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTown = town;
            }
        }

        return closestTown;
    }

    public float GetFleetHealthRatio()
    {
        var boats = fleet.GetBoats();
        if (boats == null || boats.Count == 0)
        {
            return 0f;
        }

        float current = 0f;
        float max = 0f;
        foreach (Boat b in boats)
        {
            if (b == null)
            {
                continue;
            }

            current += Mathf.Max(0, b.currentBoatHealth);
            max += Mathf.Max(1, b.maxBoatHealth);
        }

        if (max <= 0f)
        {
            return 0f;
        }

        return current / max;
    }

    #endregion
}
