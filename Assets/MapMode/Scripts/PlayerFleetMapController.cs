using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class PlayerFleetMapController : MonoBehaviour
{
    #region variables
    [System.Serializable]
    public struct PlayerFleetData
    {
        public Fleet fleet;
        public float[] pos;
    }

    public GameObject Canvas;
    private MeetShipUI meetShipUI;
    private TownUI townUI;
    private TownOptionsUI townOptionsUI;

    public NavMeshAgent navAgent;
    public Transform target;
    public static Town currentTown;
    Fleet fleet;
    
    [TextArea]
    public string boatNames = "";
    #endregion

    #region Monobehaviours
    private void Awake()
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    void Start()
    {

        GameEvents.SaveInitiated += SaveFleet;// add events to happen when save and load is called
        GameEvents.LoadInitiated += LoadFleet;

        //UpdateBoatNames();
        fleet = new Fleet("House Of Ball","Andersen");
        fleet.AddBoat(new Boat("Hogger2", "Frigate"));
        fleet.AddBoat(new Boat("Hogger3", "Frigate"));
        fleet.AddBoat(new Boat("Hogger4", "Frigate"));
        fleet.AddBoat(new Boat("Floater", "TradeShip"));
        PlayerGlobal.playerBoat = new Boat("Serpent", "Frigate");
        PlayerGlobal.money = 100000;

        BoatAILead.RemoveID(fleet.FleetID);
        
        navAgent.speed = fleet.CalculateSpeed();
        
        UpdateBoatNames();

        meetShipUI = Canvas.transform.Find("MeetShip").GetComponent<MeetShipUI>();
        townUI = GameObject.Find("ShopPanel").GetComponent<TownUI>();
        townOptionsUI = GameObject.Find("TownOptions").GetComponent<TownOptionsUI>();

    }
    private void Update()
    {
        // TODO 12/28 performance improvement is possible here. Does not need to be on every frame. Can be on event. 
        UpdateBoatNames();
        
        if (target != null)
        {
            navAgent.SetDestination(target.position);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Town town = other.GetComponent<Town>();
        FleetMapController otherFleet = other.GetComponent<FleetMapController>();
        if (otherFleet != null) {
            meetShipUI.ContactShip(otherFleet.GetFleet());
            Debug.Log("Fleet: " + fleet.commander + " contacted: " + other.transform.name + otherFleet.GetFleet().commander);

        }
        else if (town != null) {
            //           Debug.Log("Fleet: " + fleet.commander + " contacted town: " + other.transform.name);
            //           townUI.DisplayTownUI(other.GetComponent<Town>(), fleet);
            townOptionsUI.DisplayOptionsMenu(other.GetComponent<Town>());
            currentTown = other.GetComponent<Town>();
        }
    }
    #endregion

    #region Methods
    public Fleet GetFleet() { return fleet; }

    public void SetFleet(Fleet f) { 
        fleet = f;
        UpdateBoatNames();
    }

    public void DockFleet(Town town)
    {
        town.DockFleet(fleet);
        Destroy(gameObject, 1);
    }

    #endregion

    #region Developer methods
    public void UpdateBoatNames()
    {
        boatNames = ""+ fleet.commander;
        boatNames += " FleetID: " + fleet.FleetID+"\n";
        boatNames += "Flag ship:" + PlayerGlobal.playerBoat.boatName + " hp:" + PlayerGlobal.playerBoat.GetBoatHealth();
        boatNames += "\n";
        foreach (Boat b in fleet.GetBoats()) {
            boatNames += b.boatName + " hp:"+b.GetBoatHealth() +" iteams: ";
            foreach (KeyValuePair<string, int> iteam in b.getSupplies()) {
                boatNames += iteam.Key + iteam.Value + ",";
            }
            boatNames += "\n";
        }
    }

    public void SaveFleet()
    {
        PlayerFleetData saveFleet = new PlayerFleetData();
        saveFleet.fleet = fleet;
        Vector3 place = gameObject.transform.position;
        saveFleet.pos = new float[] { place.x, place.y, place.z };
        SaveLoad.Save(saveFleet, "Player");
    }
    public void LoadFleet()
    {
        if (SaveLoad.SaveExists("Player")) {
            PlayerFleetData playerData = SaveLoad.Load<PlayerFleetData>("Player");
            fleet = playerData.fleet;
            navAgent.speed = fleet.CalculateSpeed();
            Vector3 targetPosition = new(playerData.pos[0], playerData.pos[1], playerData.pos[2]);
            gameObject.transform.position = targetPosition;
            navAgent.Warp(targetPosition);

        }
        UpdateBoatNames();
    }

    #endregion

}
