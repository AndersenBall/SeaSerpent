using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Fleet
{
    public Sailor commander;
    public int FleetSizeLimit = 3;// how many boats someone can control before they get penalized

    public Boat FlagShip;
    public Nation Nationality = Nation.Britain;
    public float fleetSpeed = 10;
    public float fleetAcceleration = 10;
    public float diminishingFactorSpeed = 1;
    public float diminishingFactorAcceleration = 1;
    public AIFleetType FleetType = AIFleetType.Trade;

    [SerializeField]
    private List<Boat> boats = new List<Boat>();

    public int FleetID { get; private set; }
    public string CommanderName => commander?.Name ?? "Unknown Commander";
    public string FlagShipName => FlagShip?.boatName ?? "None";

    public Fleet(Nation natio, string commanderName, AIFleetType fleetType = AIFleetType.Trade)
        : this(natio, new Sailor(commanderName, SailorType.Captain), fleetType)
    {
    }

    public Fleet(Nation natio, Sailor fleetCommander, AIFleetType fleetType = AIFleetType.Trade)
    {
        Nationality = natio;
        commander = fleetCommander;
        FleetType = fleetType;
        FleetID = BoatAILead.AssignID();
    }

    public int getNumberBoats()
    {
        return boats.Count;
    }

    public bool AddBoat(Boat b)
    {
        foreach (Boat a in boats)
        {
            if (a.boatName == b.boatName)
            {
                return false;
            }
        }

        boats.Add(b);
        if (FlagShip == null)
        {
            FlagShip = b;
        }

        CalculateSpeed();
        return true;
    }

    public void RemoveBoat(Boat b)
    {
        Boat toRemove = null;
        foreach (Boat a in boats)
        {
            if (a.boatName == b.boatName)
            {
                toRemove = a;
                Debug.Log("boat removed. Fleet:" + CommanderName + " boat:" + b.boatName);
                break;
            }
        }

        if (toRemove != null)
        {
            boats.Remove(toRemove);
            if (FlagShip != null && FlagShip.boatName == toRemove.boatName)
            {
                FlagShip = boats.FirstOrDefault();
            }
        }

        CalculateSpeed();
    }

    public void RemoveBoat(string b)
    {
        Boat toRemove = null;
        foreach (Boat a in boats)
        {
            if (a.boatName == b)
            {
                toRemove = a;
                Debug.Log("boat removed. Fleet:" + CommanderName + " boat:" + b);
                break;
            }
        }

        if (toRemove != null)
        {
            boats.Remove(toRemove);
            if (FlagShip != null && FlagShip.boatName == toRemove.boatName)
            {
                FlagShip = boats.FirstOrDefault();
            }
        }

        CalculateSpeed();
    }

    public bool HasBoatWithName(string name)
    {
        return boats.Exists(boat => boat.boatName == name);
    }

    public bool SetFlagShip(string boatName)
    {
        Boat found = boats.FirstOrDefault(boat => boat.boatName == boatName);
        if (found == null)
        {
            return false;
        }

        FlagShip = found;
        return true;
    }

    public bool SetFlagShip(Boat boat)
    {
        if (boat == null)
        {
            return false;
        }

        return SetFlagShip(boat.boatName);
    }

    public Boat GetFlagShip()
    {
        if (FlagShip == null && boats.Count > 0)
        {
            FlagShip = boats[0];
        }

        return FlagShip;
    }

    public (float fleetSpeed, float fleetAcceleration) CalculateSpeed()
    {
        diminishingFactorSpeed = 1;

        int effectiveFleetPenaltySize = boats.Count - FleetSizeLimit;

        if (effectiveFleetPenaltySize > 0)
        {
            diminishingFactorSpeed = Mathf.Max(0.1f, -Mathf.Pow(effectiveFleetPenaltySize / 16f, 2) + 1f);
            diminishingFactorAcceleration = Mathf.Max(0.075f, -Mathf.Pow(effectiveFleetPenaltySize / 8f, 2) + 1f);
        }

        float slowestSpeed = float.MaxValue;
        float slowestAcceleration = float.MaxValue;

        foreach (Boat boat in boats)
        {
            float boatSpeed = boat.baseStats.speed;
            float boatAcceleration = boat.baseStats.turnSpeed;

            if (boatSpeed < slowestSpeed)
            {
                slowestSpeed = boatSpeed;
            }

            if (boatAcceleration < slowestAcceleration)
            {
                slowestAcceleration = boatAcceleration;
            }
        }

        fleetSpeed = 6 * slowestSpeed * diminishingFactorSpeed;
        fleetAcceleration = 400 * slowestAcceleration * diminishingFactorAcceleration;

        return (fleetSpeed, fleetAcceleration);
    }

    public string ItemBeingCarried()
    {
        foreach (Boat b in boats)
        {
            IDictionary<string, int> sup = b.getSupplies();
            foreach (KeyValuePair<string, int> ite in sup)
            {
                return ite.Key;
            }
        }
        return "None";
    }

    public (string[], int[]) GetInventory()
    {
        string[] itemNames = new string[10] { "fish", "lumber", "fur", "guns", "sugar", "coffee", "salt", "tea", "tobacco", "cotton" };

        int[] count = new int[10];
        Debug.Log("checking items:");
        foreach (Boat b in boats)
        {
            Debug.Log("boat:" + b.boatName + b.getSupplies().Count);
            foreach (KeyValuePair<string, int> item in b.getSupplies())
            {
                Debug.Log("ITEM:" + item.Key + item.Value);
                for (int i = 0; i < itemNames.Length; i++)
                {
                    if (itemNames[i] == item.Key)
                    {
                        count[i] += item.Value;
                    }
                }
            }
        }

        return (itemNames, count);
    }

    public List<Boat> GetBoats()
    {
        return boats;
    }

    public void SetBoats(List<Boat> b)
    {
        boats = b ?? new List<Boat>();
        if (boats.Count == 0)
        {
            FlagShip = null;
        }
        else if (FlagShip == null || !boats.Any(boat => boat.boatName == FlagShip.boatName))
        {
            FlagShip = boats[0];
        }
    }

    public override string ToString()
    {
        string boatsString = string.Join(", ", boats.Select(boat => boat.boatName));

        return "Fleet Information:\n" +
               $"- Commander: {CommanderName}\n" +
               $"- Commander Origin: {commander?.OriginalOccupation}\n" +
               $"- Nationality: {Nationality}\n" +
               $"- Fleet Speed: {fleetSpeed:F2}\n" +
               $"- Diminishing Factor: {diminishingFactorSpeed:F2}\n" +
               $"- Diminishing Acc Factor: {diminishingFactorAcceleration:F2}\n" +
               $"- Fleet Size Limit: {FleetSizeLimit}\n" +
               $"- Fleet Type: {FleetType}\n" +
               $"- Flagship: {FlagShipName}\n" +
               $"- Boats: [{boatsString}]";
    }
}
