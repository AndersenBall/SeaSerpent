using System.Collections.Generic;
using GerneralScripts.MissionSystem.Reward;
using GerneralScripts.MissionSystem.Reward.Templates;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(menuName = "Missions/Mission Template")]
[System.Serializable]
public class MissionTemplate : ScriptableObject
{
    [SerializeField] private string missionID;
    [SerializeField] private string title;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private List<TaskTemplate> tasks = new List<TaskTemplate>();
    [SerializeField] private List<RewardTemplate> rewards = new();

    public string MissionID => missionID;
    public string Title => title;
    public string Description => description;
    public IReadOnlyList<TaskTemplate> Tasks => tasks;
    
    public Mission BuildMission()
    {
        var runtimeTasks = new List<MissionTask>(tasks.Count);
        var runtimeRewards = new List<IMissionReward>(rewards.Count);
        foreach (var def in tasks)
        {
            var task = def.CreateRuntimeTask();
            runtimeTasks.Add(task);
        }

        foreach (var reward in rewards){
            var r = reward.CreateRuntimeReward();
            runtimeRewards.Add(r);
        }

        return new Mission(missionID, title, description, runtimeTasks,runtimeRewards);
    }

}