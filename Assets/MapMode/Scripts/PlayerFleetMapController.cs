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
    
    [TextArea]
    public string _boatNames = "";
    public string boatNames{get => _boatNames; set => _boatNames = value;}
    #endregion

    #region Monobehaviours
    private void Awake()
    {

        navAgent = gameObject.GetComponent<NavMeshAgent>();
        GameEvents.SaveInitiated += SaveFleet;
        GameEvents.LoadInitiated += LoadFleet;
    }

    void Start()
    {

        //UpdateBoatNames();
        SceneTransfer.playerFleet = new Fleet("House Of Ball","Andersen");
        SceneTransfer.playerFleet.AddBoat(new Boat("Hogger2", BoatType.Frigate));
        SceneTransfer.playerFleet.AddBoat(new Boat("Hogger3", BoatType.Frigate));
        SceneTransfer.playerFleet.AddBoat(new Boat("Hogger4", BoatType.Frigate));
        SceneTransfer.playerFleet.AddBoat(new Boat("Floater", BoatType.TradeShip));
        PlayerGlobal.playerBoat = new Boat("Serpent", BoatType.Frigate);
        PlayerGlobal.money = 5000000;

        BoatAILead.RemoveID(SceneTransfer.playerFleet.FleetID);
        
        navAgent.speed = SceneTransfer.playerFleet.CalculateSpeed();
        
        UpdateBoatNames();

        meetShipUI = Canvas.transform.Find("MeetShip").GetComponent<MeetShipUI>();
        townUI = GameObject.Find("ShopPanel").GetComponent<TownUI>();
        townOptionsUI = GameObject.Find("TownOptions").GetComponent<TownOptionsUI>();

    }
    private void Update()
    {
        // TODO 12/28 performance improvement is possible here. Does not need to be on every frame. Can be on event. 
        //UpdateBoatNames();
        
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
            Debug.Log("Fleet: " + SceneTransfer.playerFleet.commander + " contacted: " + other.transform.name + otherFleet.GetFleet().commander);

        }
        else if (town != null) {
            //           Debug.Log("Fleet: " + fleet.commander + " contacted town: " + other.transform.name);
            //           townUI.DisplayTownUI(other.GetComponent<Town>(), fleet);
            townOptionsUI.DisplayOptionsMenu(town);
            currentTown = town;
            GameEvents.SaveGame();
        }
    }
    #endregion

    #region Methods
    public Fleet GetFleet() { return SceneTransfer.playerFleet; }



    public void DockFleet(Town town)
    {
        town.DockFleet(SceneTransfer.playerFleet);
        Destroy(gameObject, 1);
    }

    #endregion

    #region Developer methods
    public void UpdateBoatNames()
    {
        boatNames = ""+ SceneTransfer.playerFleet.commander;
        boatNames += " FleetID: " + SceneTransfer.playerFleet.FleetID+"\n";
        boatNames += "Flag ship:" + PlayerGlobal.playerBoat.boatName + " hp:" + PlayerGlobal.playerBoat.GetBoatHealth();
        boatNames += "\n";
        foreach (Boat b in SceneTransfer.playerFleet.GetBoats()) {
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
        saveFleet.fleet = SceneTransfer.playerFleet;
        Vector3 place = gameObject.transform.position;
        saveFleet.pos = new float[] { place.x, place.y, place.z };
        SaveLoad.Save(saveFleet, "Player");
    }
    public void LoadFleet()
    {
        if (SaveLoad.SaveExists("Player")) {
            PlayerFleetData playerData = SaveLoad.Load<PlayerFleetData>("Player");
            SceneTransfer.playerFleet = playerData.fleet;
            navAgent.speed = SceneTransfer.playerFleet.CalculateSpeed();
            Vector3 targetPosition = new(playerData.pos[0], playerData.pos[1], playerData.pos[2]);
            gameObject.transform.position = targetPosition;
            navAgent.Warp(targetPosition);

        }
        UpdateBoatNames();
    }

    private void OnDestroy()
    {
        GameEvents.SaveInitiated -= SaveFleet;
        GameEvents.LoadInitiated -= LoadFleet;
    }


    #endregion

}
