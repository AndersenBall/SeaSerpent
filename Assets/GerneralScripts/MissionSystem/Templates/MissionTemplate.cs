using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Missions/Mission Template")]
[System.Serializable]
public class MissionTemplate : ScriptableObject
{
    [SerializeField] private string missionID;
    [SerializeField] private string title;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private List<TaskTemplate> tasks = new List<TaskTemplate>();

    public string MissionID => missionID;
    public string Title => title;
    public string Description => description;
    public IReadOnlyList<TaskTemplate> Tasks => tasks;
    
    public Mission BuildMission()
    {
        var runtimeTasks = new List<MissionTask>(tasks.Count);
        foreach (var def in tasks)
        {
            var task = def.CreateRuntimeTask();
            runtimeTasks.Add(task);
        }

        return new Mission(missionID, title, description, runtimeTasks);
    }

}