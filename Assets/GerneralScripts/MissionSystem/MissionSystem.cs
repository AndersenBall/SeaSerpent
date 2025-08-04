using System;
using System.Collections.Generic;
using MapMode.Scripts;
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
    //to delete
    private void Start()
    {
        //// Create a basic mission to kill 5 goblins
        //var goblinKillTask = new EnemyKillTask("Kill 5 Goblins", 0, 5, "Goblin");
        //var humanKillTask = new EnemyKillTask("Kill 5 Humans", 0, 5, "Human");
        //var goblinKillTask2 = new EnemyKillTask("Kill 5 Goblins", 1, 5, "Goblin");
        //var mission = new Mission("M001", "Goblin Slayer", "Defeat 5 goblins.", new List<MissionTask> { goblinKillTask, humanKillTask, goblinKillTask2 });
        //AddMission(mission);

        //Debug.Log("Added basic mission: Kill 5 Goblins");
    }

    private void Update()
    {
        // Press B to simulate killing a goblin
        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Killed a British Fleet!");
            CombatEvents.InvokeEnemyKilled("BritishFleet");
   
        }

        // Press N to simulate killing a human
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Killed a French Fleet!");

            CombatEvents.InvokeEnemyKilled("FrenchFleet");
        }

        if (Input.GetKey(KeyCode.M))
        {
            Debug.Log("Active Missions:");
            foreach (var mission in activeMissions)
            {
                Debug.Log(mission.ToString());
            }

            Debug.Log("Completed Missions:");
            foreach (var mission in completedMissions)
            {
                Debug.Log(mission.ToString());
            }


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
    private void OnDestroy()
    {
        foreach (var mission in activeMissions)
        {
            mission.Cleanup(); // Ensure cleanup is done if MissionSystem is destroyed
        }
    }

    public void AddMission(Mission mission)
    {
        if (!activeMissions.Exists(m => m.MissionID == mission.MissionID))
        {
            activeMissions.Add(mission);
            Debug.Log("Add a mission" + mission);
        }
        else {
            Debug.Log("mission already added" + mission);
        }
       
    }

    public void CompleteMission(Mission completedMission)
    {
        if (completedMission == null)
        {
            Debug.LogWarning("Attempted to complete a null mission.");
            return;
        }

        if (!activeMissions.Contains(completedMission))
        {
            Debug.LogWarning($"Mission '{completedMission.Title}' is not in the active missions list.");
            return;
        }

        Debug.Log($"Completed a mission: {completedMission.Title}\nDetails: {completedMission}");

        completedMission.Cleanup(); // Ensure events are unsubscribed
        activeMissions.Remove(completedMission); // Safely remove the mission from the active list
        completedMissions.Add(completedMission); // Add the mission to the completed list

        Debug.Log($"Mission '{completedMission.Title}' moved to completed missions.");
    }

    public void Save()
    {
        MissionSaveData saveData = new MissionSaveData
        {
            ActiveMissions = activeMissions,
            CompletedMissions = completedMissions
        };
        Debug.Log("save the missions");
        SaveLoad.Save(saveData, SaveKey);
    }

    public void Load()
    {
        if (SaveLoad.SaveExists(SaveKey))
        {
            // Clear and deconstruct existing missions
            foreach (var mission in activeMissions)
            {
                mission.Cleanup(); // Ensure events are unsubscribed
            }
            activeMissions.Clear();

            foreach (var mission in completedMissions)
            {
                mission.Cleanup(); // Ensure events are unsubscribed
            }
            completedMissions.Clear();
        
            MissionSaveData saveData = SaveLoad.Load<MissionSaveData>(SaveKey);
            // Add active missions using AddMission for consistent initialization
            foreach (var mission in saveData.ActiveMissions )
            {
                mission.Initialize();
                AddMission(mission);
            }

            // Add completed missions directly since they are not active
            completedMissions = saveData.CompletedMissions ;

            Debug.Log("Missions loaded successfully.");
        }
        else
        {
            Debug.LogWarning("No save file found to load missions.");
        }
    }

}
