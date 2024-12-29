using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public static bool RemoveID(int id)
    {
        if (!boatIDs.Contains(id))
        {
            Debug.LogWarning($"Attempted to remove FleetID {id}, but it was not found in boatIDs.");
            return false; 
        }

        Debug.Log($"Fleet removed: {id}");
        return boatIDs.Remove(id); 
    }


    public void LoadAllBoats() {
        
        Transform parent = GameObject.Find("/Boats").transform;
        for (int i = 0; i < parent.childCount; i++) {
            Destroy(parent.GetChild(i).gameObject);
        }

        FleetMapData data = SaveLoad.Load<FleetMapData>("CurrentID");
        currentID = data.currentID;
        boatIDs = new List<int>();

        string saveDirectory = Application.persistentDataPath + "/saves/";
        if (!Directory.Exists(saveDirectory))
        {
            Debug.LogWarning("Save directory not found: " + saveDirectory);
            return;
        }

        string[] boatFiles = Directory.GetFiles(saveDirectory, "boat*.txt");
        foreach (string filePath in boatFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath); 
            if (fileName.StartsWith("boat") && int.TryParse(fileName.Substring(4), out int boatID))
            {
                if (!boatIDs.Contains(boatID)) 
                {
                    boatIDs.Add(boatID);
                }
                // Instantiate the boat
                GameObject fleetPrefab = Instantiate(prefabFleet, transform.position, Quaternion.identity, parent);
                fleetPrefab.GetComponent<FleetMapController>().LoadNPCFleet(boatID);
            }
            else
            {
                Debug.LogWarning("Invalid boat file name: " + fileName);
            }
        }

    }
    
    public void SaveIDs() {
        FleetMapData data = new FleetMapData();
        data.currentID = currentID;
        data.boatIDs = boatIDs;
        SaveLoad.Save(data, "CurrentID");
    }
    
}
