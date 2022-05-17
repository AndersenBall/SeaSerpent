using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public Unit pathfinding;
    Fleet fleet;
    
    [TextArea]
    public string boatNames = "";
    #endregion

    #region Monobehaviours
    void Start()
    {

        GameEvents.SaveInitiated += SaveFleet;// add events to happen when save and load is called
        GameEvents.LoadInitiated += LoadFleet;

        //UpdateBoatNames();
        fleet = new Fleet("House Of Ball","Andersen");
        fleet.AddBoat(new Boat("Hogger2", "Frigate"));
        fleet.AddBoat(new Boat("Floater", "TradeShip"));
        PlayerGlobal.playerBoat = new Boat("Serpent", "Frigate");
        PlayerGlobal.money = 100000;

        BoatAILead.RemoveID(fleet.FleetID);

        pathfinding = GetComponent<Unit>();//get unit attached 
        pathfinding.speed = fleet.CalculateSpeed(); // set speed based on fleet
        UpdateBoatNames();

        meetShipUI = Canvas.transform.Find("MeetShip").GetComponent<MeetShipUI>();

    }
    private void Update()
    {
        UpdateBoatNames();
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
            Debug.Log("Fleet: " + fleet.commander + " contacted town: " + other.transform.name);
            TownUI townUI = GameObject.Find("ShopPanel").GetComponent<TownUI>();
            townUI.DisplayTownUI(other.GetComponent<Town>(), fleet);
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
            pathfinding.speed = fleet.CalculateSpeed();
            gameObject.transform.position = new Vector3(playerData.pos[0], playerData.pos[1], playerData.pos[2]);
        }
        UpdateBoatNames();
    }

    #endregion

}
