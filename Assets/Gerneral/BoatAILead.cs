using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatAILead : MonoBehaviour
{

    [System.Serializable]
    public struct FleetMapData {
        public int currentID;
        public List<int> boatIDs;
    }
    private static int currentID = -1;
    private static List<int> boatIDs = new List<int>();

    public GameObject prefabFleet;
    void Start() {
        GameEvents.SaveInitiated += SaveIDs;
        
        GameEvents.LoadInitiated += LoadAllBoats;
    }



    public static int AssignID() {
        currentID += 1;
        //Debug.Log("Log:BoatAIStatic:current ID" + currentID);
        boatIDs.Add(currentID);
        return currentID;
    }
    public static bool RemoveID(int id) {
        Debug.Log("Fleet removed:" + id);
       
        return boatIDs.Remove(id);
    }

    public void LoadAllBoats() {
        
        Transform parent = GameObject.Find("/Boats").transform;
        for (int i = 0; i < parent.childCount; i++) {
            parent.GetChild(i).gameObject.GetComponent<FleetMapController>().DestroyFleet();
        }

        FleetMapData data = SaveLoad.Load<FleetMapData>("CurrentID");
        currentID = data.currentID;
        boatIDs = data.boatIDs;
        foreach (int boatID in BoatAILead.boatIDs) {
        //Debug.Log("load boat" + boatID);
            GameObject fleetPrefab = Instantiate(prefabFleet, transform.position, Quaternion.identity, parent);
            fleetPrefab.GetComponent<FleetMapController>().LoadNPCFleet(boatID);  
        }   
    }
    
    public void SaveIDs() {
        FleetMapData data = new FleetMapData();
        data.currentID = currentID;
        data.boatIDs = boatIDs;
        SaveLoad.Save(data, "CurrentID");
    }
    
}
