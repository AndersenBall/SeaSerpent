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
        InitializeDefaultsIfNeeded();

        BoatAILead.RemoveID(PlayerStateService.PlayerFleet.FleetID);

        (float fleetSpeed, float fleetAcceleration) = PlayerStateService.PlayerFleet.CalculateSpeed();
        navAgent.speed = fleetSpeed;
        navAgent.acceleration = fleetAcceleration;

        UpdateBoatNames();

        meetShipUI = Canvas.transform.Find("MeetShip").GetComponent<MeetShipUI>();
        townUI = GameObject.Find("ShopPanel").GetComponent<TownUI>();
        townOptionsUI = GameObject.Find("TownOptions").GetComponent<TownOptionsUI>();

    }
    private void Update()
    {
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
            Debug.Log("Fleet: " + PlayerStateService.PlayerFleet.CommanderName + " contacted: " + other.transform.name + otherFleet.GetFleet().CommanderName);

        }
        else if (town != null) {
            townOptionsUI.DisplayOptionsMenu(town);
            currentTown = town;
            TownEvents.InvokeTownVisited(town);
            GameEvents.SaveGame();
        }
    }
    #endregion

    #region Methods
    public Fleet GetFleet() { return PlayerStateService.PlayerFleet; }

    public void DockFleet(Town town)
    {
        town.DockFleet(PlayerStateService.PlayerFleet);
        Destroy(gameObject, 1);
    }

    #endregion

    #region Developer methods
    public void UpdateBoatNames()
    {
        boatNames += PlayerStateService.PlayerFleet.ToString();

    }

    public void SaveFleet()
    {
        if (PlayerStateService.PlayerFleet == null)
        {
            return;
        }

        PlayerStateService.PlayerFleet.CalculateSpeed();
        PlayerStateService.MapPosition = new float[]
        {
            transform.position.x,
            transform.position.y,
            transform.position.z
        };
    }

    public void LoadFleet()
    {
        if (PlayerStateService.PlayerFleet == null)
        {
            return;
        }

        navAgent.speed = PlayerStateService.PlayerFleet.fleetSpeed;
        navAgent.acceleration = PlayerStateService.PlayerFleet.fleetAcceleration;

        float[] pos = PlayerStateService.MapPosition;
        if (pos != null && pos.Length >= 3)
        {
            Vector3 targetPosition = new(pos[0], pos[1], pos[2]);
            gameObject.transform.position = targetPosition;
            navAgent.Warp(targetPosition);
        }

        UpdateBoatNames();
    }

    private void InitializeDefaultsIfNeeded()
    {
        if (PlayerStateService.PlayerFleet == null)
        {
            PlayerStateService.PlayerFleet = new Fleet(Nation.PlayerNation, "Andersen");
            PlayerStateService.PlayerFleet.AddBoat(new Boat("Hogger2", BoatType.Frigate));
            PlayerStateService.PlayerFleet.AddBoat(new Boat("Floater", BoatType.TradeShip));
        }

        if (PlayerStateService.Money <= 0)
        {
            PlayerStateService.Money = 5000000;
        }
    }

    private void OnDestroy()
    {
        GameEvents.SaveInitiated -= SaveFleet;
        GameEvents.LoadInitiated -= LoadFleet;
    }


    #endregion

}
