using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Missions/Mission Template")]
public class MissionTemplate : ScriptableObject
{
    [SerializeField] private string missionID;
    [SerializeField] private string title;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private List<TaskDefinition> tasks = new List<TaskDefinition>();

    public string MissionID => missionID;
    public string Title => title;
    public string Description => description;
    public IReadOnlyList<TaskDefinition> Tasks => tasks;
}