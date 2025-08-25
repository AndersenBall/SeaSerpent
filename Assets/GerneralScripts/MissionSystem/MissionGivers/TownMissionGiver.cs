using System.Collections.Generic;
using UnityEngine;
using GerneralScripts.MissionSystem;

public class TownMissionGiverSaveData
{
    public List<MissionTemplate> availableMissions;
    public HashSet<string> _assignedMissionIds;
}

public class TownMissionGiver : MonoBehaviour
{
    private Town town;
    
    [Header("Available Missions For This Town")]
    [SerializeField] private List<MissionTemplate> availableMissions = new ();
    
    private HashSet<string> _assignedMissionIds = new();

    private void Awake()
    {
        town = GetComponent<Town>();
        if (town == null)
        {
            Debug.LogError($"[{nameof(TownMissionGiver)}] No Town component found on GameObject.");
        }
    }

    private MissionTemplate FindMissionTemplateByName(string missionID)
    {
        return availableMissions.Find(m => m.MissionID == missionID && !_assignedMissionIds.Contains(m.MissionID));
    }

    public void AssignMissionByName(string missionID)
    {
        if (availableMissions == null || availableMissions.Count == 0)
        {
            Debug.LogWarning($"[{nameof(TownMissionGiver)}] No available missions for town '{town?.name ?? "Unknown"}'.");
            return;
        }

        var template = FindMissionTemplateByName(missionID);
        if (template == null)
        {
            Debug.LogWarning($"[{nameof(TownMissionGiver)}] Mission '{missionID}' not found or already assigned in town '{town?.name ?? "Unknown"}'.");
            return;
        }

        var mission = MissionSystem.Instance.CreateMission(template);
        _assignedMissionIds.Add(mission.MissionID);
    }

    #region  save load
    
    private const string SaveKeyPrefix = "TownMissions_";
    private string SaveKey => SaveKeyPrefix + (town?.name ?? "Unknown");

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

    public void Save()
    {
        var saveData = new TownMissionGiverSaveData();
        saveData.availableMissions = availableMissions;
        saveData._assignedMissionIds = _assignedMissionIds;
        
        SaveLoad.Save(saveData, SaveKey);
        Debug.Log($"[{nameof(TownMissionGiver)}] Saved missions for town '{name}'.");
    }

    public void Load()
    {
        if (SaveLoad.SaveExists(SaveKey))
        {
            TownMissionGiverSaveData saveData = SaveLoad.Load<TownMissionGiverSaveData>(SaveKey);

            availableMissions = saveData.availableMissions;
            _assignedMissionIds = saveData._assignedMissionIds;

            Debug.Log($"[{nameof(TownMissionGiver)}] Loaded missions for town '{name}'.");
        }
        else
        {
            Debug.LogWarning($"[{nameof(TownMissionGiver)}] No save data found for town '{town?.name ?? "Unknown"}'.");
        }
    }

    #endregion
}
