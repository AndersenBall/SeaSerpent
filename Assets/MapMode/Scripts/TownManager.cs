using System;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    [Header("Trade")]
    public float baseCostPerShip = 100000;
    public float costPerUnitDistance = 100;

    [Header("AI Fleet Spawning")]
    public bool enablePirateSpawns = true;
    public bool enableNavalPatrolSpawns = true;
    public bool enableWarFleetSpawns = false;

    public float pirateSpawnInterval = 45f;
    public float navalPatrolSpawnInterval = 60f;
    public float warFleetSpawnInterval = 90f;

    [Range(1, 6)] public int pirateFleetSize = 2;
    [Range(1, 6)] public int patrolFleetSize = 2;
    [Range(1, 8)] public int warFleetSize = 3;

    public Town[] towns { get; set; }
    public IDictionary<string, float> standardPrices = new Dictionary<string, float>();

    private float _pirateSpawnTimer;
    private float _navalPatrolSpawnTimer;
    private float _warFleetSpawnTimer;

    void Start()
    {
        towns = GetComponentsInChildren<Town>();
        standardPrices.Add("fish", 100);
        standardPrices.Add("lumber", 150);
        standardPrices.Add("fur", 2000);
        standardPrices.Add("guns", 2500);
        standardPrices.Add("sugar", 100);
        standardPrices.Add("coffee", 180);
        standardPrices.Add("salt", 150);
        standardPrices.Add("tea", 200);
        standardPrices.Add("tobacco", 400);
        standardPrices.Add("cotton", 150);
    }

    private void Update()
    {
        if (towns == null || towns.Length == 0)
        {
            return;
        }

        HandleAIFleetSpawning();
    }

    public (Fleet, int) RequestItemNonSurplus(string item, int amount, Town originTown)
    {
        if (originTown == null)
        {
            throw new ArgumentNullException(nameof(originTown), "Origin town cannot be null.");
        }

        float highestProfit = 0;
        int lessAmount = int.MaxValue;
        Town chosenTown = null;

        foreach (Town t in towns)
        {
            if (t.name == originTown.name)
            {
                continue;
            }

            int equalAmount = BlanceResourceAmount(t, originTown, item);

            if (equalAmount <= 0)
            {
                continue;
            }

            equalAmount = Math.Max(equalAmount, ((equalAmount - 20) / 50) * 50);
            float transportationCost = JourneyCost(t, originTown, equalAmount);
            float profitGoods = originTown.CalculateTransactionPrice(item, equalAmount);
            float cost = -t.CalculateTransactionPrice(item, -equalAmount);
            float totalProfit = profitGoods - transportationCost - cost;

            if (totalProfit > highestProfit)
            {
                highestProfit = totalProfit;
                chosenTown = t;
                lessAmount = equalAmount;
                Debug.Log("item:" + item + " cost:" + cost + "journy cost:" + transportationCost + "profit Goods:" + profitGoods + "totalprofit:" + totalProfit + this.name);
            }
        }

        amount = Mathf.Min(amount, lessAmount);

        if (chosenTown != null)
        {
            Fleet fleet = chosenTown.MakeTradeFleet(item, amount);
            if (fleet == null)
            {
                Debug.LogError($"Failed to create a fleet for item: {item}, amount: {amount}." + this.name);
                return (null, -1);
            }

            chosenTown.SendOutFleet(fleet, originTown.transform);
            return (fleet, amount);
        }

        return (null, -1);
    }

    public bool TrySpawnFleet(AIFleetType fleetType)
    {
        switch (fleetType)
        {
            case AIFleetType.Pirate:
                return TrySpawnPirateFleet();
            case AIFleetType.NavalPatrol:
                return TrySpawnNavalPatrolFleet();
            case AIFleetType.War:
                return TrySpawnWarFleet();
            default:
                return false;
        }
    }

    private void HandleAIFleetSpawning()
    {
        _pirateSpawnTimer += Time.deltaTime;
        _navalPatrolSpawnTimer += Time.deltaTime;
        _warFleetSpawnTimer += Time.deltaTime;

        if (enablePirateSpawns && _pirateSpawnTimer >= pirateSpawnInterval)
        {
            if (TrySpawnPirateFleet())
            {
                _pirateSpawnTimer = 0f;
            }
        }

        if (enableNavalPatrolSpawns && _navalPatrolSpawnTimer >= navalPatrolSpawnInterval)
        {
            if (TrySpawnNavalPatrolFleet())
            {
                _navalPatrolSpawnTimer = 0f;
            }
        }

        if (enableWarFleetSpawns && _warFleetSpawnTimer >= warFleetSpawnInterval)
        {
            if (TrySpawnWarFleet())
            {
                _warFleetSpawnTimer = 0f;
            }
        }
    }

    private bool TrySpawnPirateFleet()
    {
        FleetMapController tradeFleetTarget = FindRandomFleet(AIFleetType.Trade);
        if (tradeFleetTarget == null)
        {
            return false;
        }

        Town spawnTown = GetClosestTown(tradeFleetTarget.transform.position);
        if (spawnTown == null)
        {
            return false;
        }

        Fleet pirateFleet = spawnTown.CreateFleet(AIFleetType.Pirate, pirateFleetSize);
        spawnTown.SendOutFleet(pirateFleet, tradeFleetTarget.transform);
        return true;
    }

    private bool TrySpawnNavalPatrolFleet()
    {
        if (towns.Length < 2)
        {
            return false;
        }

        Town originTown = towns[UnityEngine.Random.Range(0, towns.Length)];
        Town destinationTown = GetRandomTown(originTown);
        if (destinationTown == null)
        {
            return false;
        }

        Fleet patrolFleet = originTown.CreateFleet(AIFleetType.NavalPatrol, patrolFleetSize);
        originTown.SendOutFleet(patrolFleet, destinationTown.transform);
        return true;
    }

    private bool TrySpawnWarFleet()
    {
        // Future-proofing for wartime fleet behavior:
        // currently just sends a heavy fleet between two random towns.
        if (towns.Length < 2)
        {
            return false;
        }

        Town originTown = towns[UnityEngine.Random.Range(0, towns.Length)];
        Town destinationTown = GetRandomTown(originTown);
        if (destinationTown == null)
        {
            return false;
        }

        Fleet warFleet = originTown.CreateFleet(AIFleetType.War, warFleetSize);
        originTown.SendOutFleet(warFleet, destinationTown.transform);
        return true;
    }

    private Town GetRandomTown(Town excludingTown)
    {
        if (towns.Length < 2)
        {
            return null;
        }

        for (int i = 0; i < 8; i++)
        {
            Town randomTown = towns[UnityEngine.Random.Range(0, towns.Length)];
            if (randomTown != null && randomTown != excludingTown)
            {
                return randomTown;
            }
        }

        foreach (Town town in towns)
        {
            if (town != excludingTown)
            {
                return town;
            }
        }

        return null;
    }

    private Town GetClosestTown(Vector3 worldPosition)
    {
        Town closestTown = null;
        float closestDistance = float.MaxValue;

        foreach (Town town in towns)
        {
            float currentDistance = (town.transform.position - worldPosition).sqrMagnitude;
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestTown = town;
            }
        }

        return closestTown;
    }

    private FleetMapController FindRandomFleet(AIFleetType fleetType)
    {
        FleetMapController[] allFleets = FindObjectsOfType<FleetMapController>();
        List<FleetMapController> matchingFleets = new List<FleetMapController>();

        foreach (FleetMapController fleetController in allFleets)
        {
            Fleet fleet = fleetController.GetFleet();
            if (fleet != null && fleet.FleetType == fleetType)
            {
                matchingFleets.Add(fleetController);
            }
        }

        if (matchingFleets.Count == 0)
        {
            return null;
        }

        return matchingFleets[UnityEngine.Random.Range(0, matchingFleets.Count)];
    }

    //amount of a resource to transfer between the 2 towns
    public int BlanceResourceAmount(Town t1, Town t2, string r)
    {
        int amount = ((t2.DemandOfItem(r) * t1.SupplyOfItem(r)) - (t1.DemandOfItem(r) * t2.SupplyOfItem(r)))
            / (t2.DemandOfItem(r) + t1.DemandOfItem(r));

        return Mathf.Max(amount, 0);
    }

    public float JourneyCost(Town t1, Town t2, int amount)
    {
        int numberOfShips = Mathf.CeilToInt((float)amount / 100);
        return numberOfShips * ((t1.transform.position - t2.transform.position).magnitude * costPerUnitDistance + baseCostPerShip);
    }
}
