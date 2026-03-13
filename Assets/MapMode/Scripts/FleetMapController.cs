using System.Collections;
using System.Collections.Generic;
using MapMode.Scripts;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

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
    }

    private void Update()
    {

        if (navAgent != null)
        {
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

        if (fleet.FleetType == AIFleetType.Pirate && otherFleet.FleetType == AIFleetType.Trade)
        {
            BattlePredicter.ApplyBattleDamage(fleet, otherFleet);
            UpdateAfterEncounter(otherFleetController);
        }
        else if (fleet.FleetType == AIFleetType.Trade && otherFleet.FleetType == AIFleetType.Pirate)
        {
            BattlePredicter.ApplyBattleDamage(otherFleet, fleet);
            otherFleetController.UpdateAfterEncounter(this);
        }
    }

    public void UpdateAfterEncounter(FleetMapController otherFleetController)
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

        if (fleet.FleetType == AIFleetType.Pirate)
        {
            FleetMapController newTarget = FindClosestTradeFleet();
            if (newTarget != null)
            {
                destination = newTarget.transform;
            }
        }
    }

    private FleetMapController FindClosestTradeFleet()
    {
        FleetMapController[] allFleets = FindObjectsOfType<FleetMapController>();
        FleetMapController closestFleet = null;
        float closestDistance = float.MaxValue;

        foreach (FleetMapController fleetController in allFleets)
        {
            if (fleetController == this)
            {
                continue;
            }

            Fleet candidate = fleetController.GetFleet();
            if (candidate == null || candidate.FleetType != AIFleetType.Trade)
            {
                continue;
            }

            float distance = (fleetController.transform.position - transform.position).sqrMagnitude;
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

    #endregion
}
