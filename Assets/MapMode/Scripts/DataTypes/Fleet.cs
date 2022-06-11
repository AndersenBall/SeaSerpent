using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fleet 
{
    public string commander;
    public string Nationality = "British";
    public float fleetSpeed = 10;
    private List<Boat> boats = new List<Boat>();
    private int fleetID;
  

    public int FleetID { get { return fleetID; } }

    public Fleet(string Natio,string command) {
        Nationality = Natio;
        commander = command;
        fleetID = BoatAILead.AssignID();
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
                CalculateSpeed();
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

    public float CalculateSpeed() {
        float total = 0;
        int count = 0;
         foreach(Boat boat in boats) {
            total += boat.GetSpeed();
             count += 1;
        
         }
         fleetSpeed = total / count;
        
         return fleetSpeed;
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

  

}
