using System.Collections;
using System.Collections.Generic;
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

    public Transform _destination;
    public Transform destination { get => _destination; set => _destination = value; }
    // Start is called before the first frame update
    Fleet fleet;
    [TextArea]
    public string boatNames = "";
    #endregion
    #region Monobehabiours

    private void Awake()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        //_destination = GetComponent<Transform>();
    }
    void Start()
    {
        GameEvents.SaveInitiated += SaveNPCFleet;

        navAgent.speed = fleet.CalculateSpeed();
        UpdateBoatNames();
    }

    private void Update()
    {
        if (destination != null)
        {
            navAgent.SetDestination(destination.position);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        Town town = other.GetComponent<Town>();
        FleetMapController otherFleet = other.GetComponent<FleetMapController>();
        if (otherFleet != null) {
            //Debug.Log("Fleet: " + fleet.commander + " contacted: " + other.transform.name + otherFleet.GetFleet().commander);
        }
        else if (town != null && town.name == destination.name) {
            //Debug.Log("Fleet: " + fleet.commander + " contacted town: " + other.transform.name);
            town.SellItemsInCargo(fleet,10000,"All");
            UpdateBoatNames();
            DockFleet(town);
        }
    }
    #endregion
    #region methods
    public void SetFleet(Fleet f) {fleet = f;}
    public Fleet GetFleet(){ return fleet;}

    public void DockFleet(Town town) {
        town.DockFleet(fleet);
        DestroyFleet();
    }
    public void DestroyFleet() {
        BoatAILead.RemoveID(fleet.FleetID);
        GameEvents.SaveInitiated -= SaveNPCFleet;
        Destroy(gameObject, .1f);
    }
    #endregion
    #region DevMethods
    public void SaveNPCFleet() {
        FleetData data = new FleetData();
        data.fleet = fleet;
        Vector3 location = gameObject.transform.position;
        data.pos = new float[] { location.x, location.y, location.z };
        data.destName = destination.name;
        SaveLoad.Save<FleetData>(data, "boat"+fleet.FleetID);
    }
    public void LoadNPCFleet(int id) {
        FleetData data = SaveLoad.Load<FleetData>("boat"+id);
        fleet = data.fleet;
        gameObject.name = "fleet" + fleet.FleetID;
        transform.position = new Vector3(data.pos[0], data.pos[1], data.pos[2]);
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
        boatNames = "";
        boatNames += "FleetID: " + fleet.FleetID + "\n";
        foreach (Boat b in fleet.GetBoats()) {
            boatNames += b.boatName + " hp:" + b.GetBoatHealth() + " iteams: ";
            foreach (KeyValuePair<string, int> iteam in b.getSupplies()) {
                boatNames += iteam.Key + iteam.Value + ",";

            }
            boatNames += "\n";
        }
    }
    #endregion
}
