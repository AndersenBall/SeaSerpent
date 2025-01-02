using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MissionSaveData
{
    public List<Mission> ActiveMissions;
    public List<Mission> CompletedMissions;
}

public class MissionSystem : MonoBehaviour
{
    public static MissionSystem Instance;

    [SerializeField]
    private List<Mission> activeMissions = new List<Mission>();

    [SerializeField]
    private List<Mission> completedMissions = new List<Mission>();

    private const string SaveKey = "MissionSystem";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        GameEvents.SaveInitiated += Save;
        GameEvents.LoadInitiated += Load;
    }

    private void OnDisable()
    {
        GameEvents.SaveInitiated -= Save;
        GameEvents.LoadInitiated -= Load;
    }

    public void AddMission(Mission mission)
    {
        if (!activeMissions.Exists(m => m.MissionID == mission.MissionID))
        {
            activeMissions.Add(mission);
            Debug.Log($"Mission added: {mission.Title}");
        }
        else
        {
            Debug.LogWarning($"Mission with ID {mission.MissionID} already exists.");
        }
    }

    public void CompleteRequirement(string missionID, string targetID, object data)
    {
        Mission mission = activeMissions.Find(m => m.MissionID == missionID);
        if (mission != null)
        {
            if (mission.CompleteRequirement(targetID, data))
            {
                if (mission.IsCompleted)
                {
                    activeMissions.Remove(mission);
                    completedMissions.Add(mission);
                    Debug.Log($"Mission completed: {mission.Title}");
                }
                else
                {
                    Debug.Log($"Progress made on mission: {mission.Title}");
                }
            }
            else
            {
                Debug.LogWarning($"Current requirements not met for mission: {mission.Title}");
            }
        }
        else
        {
            Debug.LogWarning($"Mission with ID {missionID} not found.");
        }
    }

    public void Save()
    {
        MissionSaveData saveData = new MissionSaveData
        {
            ActiveMissions = activeMissions,
            CompletedMissions = completedMissions
        };
        SaveLoad.Save(saveData, SaveKey);
        Debug.Log("MissionSystem saved.");
    }

    public void Load()
    {
        if (SaveLoad.SaveExists(SaveKey))
        {
            MissionSaveData saveData = SaveLoad.Load<MissionSaveData>(SaveKey);
            activeMissions = saveData.ActiveMissions ?? new List<Mission>();
            completedMissions = saveData.CompletedMissions ?? new List<Mission>();
            Debug.Log("MissionSystem loaded successfully.");
        }
        else
        {
            Debug.LogWarning("No MissionSystem save data found.");
        }
    }

    public void DisplayMissions()
    {
        Debug.Log("Active Missions:");
        foreach (var mission in activeMissions)
        {
            Debug.Log($"- {mission.Title}: {mission.Description} (Completed: {mission.IsCompleted})");
        }

        Debug.Log("Completed Missions:");
        foreach (var mission in completedMissions)
        {
            Debug.Log($"- {mission.Title}: {mission.Description}");
        }
    }
}
