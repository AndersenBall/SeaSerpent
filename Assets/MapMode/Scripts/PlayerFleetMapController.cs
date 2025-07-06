using System.Collections;
using System.Collections.Generic;
using MapMode.Scripts.DataTypes.boatComponents.Cannons;
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
        SceneTransfer.playerFleet.AddBoat(new Boat("Floater", BoatType.TradeShip));

        PlayerGlobal.money = 5000000;

        BoatAILead.RemoveID(SceneTransfer.playerFleet.FleetID);

        (float fleetSpeed, float fleetAcceleration) = SceneTransfer.playerFleet.CalculateSpeed();
        navAgent.speed = fleetSpeed;
        navAgent.acceleration = fleetAcceleration;

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
        boatNames += SceneTransfer.playerFleet.ToString();

    }

    public void SaveFleet()
    {
        SceneTransfer.playerFleet.CalculateSpeed();
        PlayerFleetData saveFleet = new PlayerFleetData
        {
            fleet = SceneTransfer.playerFleet, 
            pos = new float[] 
            {
            transform.position.x,
            transform.position.y,
            transform.position.z
            }
        };
        SaveLoad.Save(saveFleet, "Player");
        Debug.Log("Player Fleet saved successfully.");
    }
    public void LoadFleet()
    {
        if (SaveLoad.SaveExists("Player")) {
            PlayerFleetData playerData = SaveLoad.Load<PlayerFleetData>("Player");
            
            SceneTransfer.playerFleet = playerData.fleet;

            navAgent.speed = SceneTransfer.playerFleet.fleetSpeed;
            navAgent.acceleration = SceneTransfer.playerFleet.fleetAcceleration;
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
