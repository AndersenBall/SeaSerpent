using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public string missionType { get; set; }
    public string missionName { get; set; }
    
    string[] steps;

    /// <summary>
    /// Mission Type: Deliver, Trade, Assassination, Explore, StopBlockade, SetBlockade, WarCrime
    /// </summary>
    public Mission(string name,string type) {
        missionName = name;
        missionType = type;
    }
}
