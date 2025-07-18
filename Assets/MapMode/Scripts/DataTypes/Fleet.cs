using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Fleet 
{
    public string commander;
    public int FleetSizeLimit = 3;// how many boats someone can control before they get penalized

    public string FlagShip = "None";
    public string Nationality = "British";
    public float fleetSpeed = 10;
    public float fleetAcceleration = 10;
    public float diminishingFactorSpeed = 1;
    public float diminishingFactorAcceleration = 1;

    [SerializeField]
    private List<Boat> boats = new List<Boat>();

    public int FleetID { get; private set; }


    public Fleet(string Natio,string command) {
        Nationality = Natio;
        commander = command;
        FleetID = BoatAILead.AssignID();
    }

    public int getNumberBoats() {
        return boats.Count;
    }
    public bool AddBoat(Boat b) {
        foreach (Boat a in boats) {
            if (a.boatName == b.boatName) {
                return false;
            }
        }
        boats.Add(b);
        CalculateSpeed();
        return true;
    }
    public void RemoveBoat(Boat b) {
        Boat toRemove=null;
        foreach (Boat a in boats) {
            if (a.boatName == b.boatName) {
                toRemove = a;
                Debug.Log("boat removed. Fleet:" + commander + " boat:" + b.boatName);
                break;
            }
        }
        if(toRemove != null) boats.Remove(toRemove);
        CalculateSpeed();
    }
    public void RemoveBoat(string b)
    {
        Boat toRemove = null;
        foreach (Boat a in boats) {
            if (a.boatName == b) {
                toRemove = a;
                Debug.Log("boat removed. Fleet:" + commander + " boat:" + b);
                break;
            }
        }
        if (toRemove != null) boats.Remove(toRemove);
        CalculateSpeed();
    }

    public bool HasBoatWithName(string name)
    {
        return boats.Exists(boat => boat.boatName == name); 
    }

    public (float fleetSpeed, float fleetAcceleration) CalculateSpeed()
    {
        // Reset diminishing factor
        diminishingFactorSpeed = 1;

        // Calculate fleet penalty if over the size limit
        int effectiveFleetPenaltySize = boats.Count - FleetSizeLimit;

        if (effectiveFleetPenaltySize > 0)
        {
            diminishingFactorSpeed = Mathf.Max(0.1f, -Mathf.Pow(effectiveFleetPenaltySize / 16f, 2) + 1f);
            diminishingFactorAcceleration = Mathf.Max(0.075f, -Mathf.Pow(effectiveFleetPenaltySize / 8f, 2) + 1f);
        }

        float slowestSpeed = float.MaxValue;
        float slowestAcceleration = float.MaxValue;

        // Determine the slowest speed and acceleration in the fleet
        foreach (Boat boat in boats)
        {
            float boatSpeed = boat.baseStats.speed;
            float boatAcceleration = boat.baseStats.turnSpeed; // Assume Boat class has a GetAcceleration method

            if (boatSpeed < slowestSpeed)
            {
                slowestSpeed = boatSpeed;
            }

            if (boatAcceleration < slowestAcceleration)
            {
                slowestAcceleration = boatAcceleration;
            }
        }

        // Calculate fleet speed and acceleration
        fleetSpeed = 6 * slowestSpeed * diminishingFactorSpeed;
        fleetAcceleration = 400 * slowestAcceleration * diminishingFactorAcceleration;

        return (fleetSpeed, fleetAcceleration); // Return both as a tuple
    }


    public string ItemBeingCarried() {
        foreach (Boat b in boats) {
            IDictionary<string, int> sup = b.getSupplies();
            foreach (KeyValuePair<string, int> ite in sup) {
                return ite.Key;
            }
        }
        return "None";
    }
    public (string[], int[]) GetInventory() {
        string[] itemNames = new string[10] { "fish", "lumber", "fur", "guns", "sugar", "coffee", "salt", "tea", "tobacco", "cotton" };

        int[] count = new int[10];
        Debug.Log("checking items:");
        foreach (Boat b in boats) {
            Debug.Log("boat:" + b.boatName + b.getSupplies().Count);
            foreach (KeyValuePair<string, int> item in b.getSupplies()) {
                Debug.Log("ITEM:"+ item.Key+item.Value);
                for (int i = 0; i < itemNames.Length; i++) {
                    if (itemNames[i] == item.Key) {
                        count[i] += item.Value;
                    }
                }
            }
        
        }

        return (itemNames,count);
    }

    public List<Boat> GetBoats() {
        return boats;
    }
  
    public void SetBoats(List<Boat> b) {
        boats = b;
    }

    public override string ToString()
    {
        // Create a string representation of the boats
        string boatsString = string.Join(", ", boats.Select(boat => boat.boatName));

        // Construct the Fleet string
        return $"Fleet Information:\n" +
               $"- Commander: {commander}\n" +
               $"- Nationality: {Nationality}\n" +
               $"- Fleet Speed: {fleetSpeed:F2}\n" +
               $"- Diminishing Factor: {diminishingFactorSpeed:F2}\n" +
               $"- Diminishing Acc Factor: {diminishingFactorAcceleration:F2}\n" +
               $"- Fleet Size Limit: {FleetSizeLimit}\n" +
               $"- Boats: [{boatsString}]";
    }


}
