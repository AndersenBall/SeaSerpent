using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Top-level authoring asset for a mission. Holds mission metadata and a list of task definitions.
/// </summary>
[CreateAssetMenu(menuName = "Missions/Mission Definition")]
public class MissionDefinition : ScriptableObject
{
    [Header("Mission Metadata")]
    [SerializeField] private string missionID;
    [SerializeField] private string title;
    [TextArea]
    [SerializeField] private string description;

    [Header("Tasks (drag in MissionTaskDefinition assets)")]
    [SerializeField] private List<MissionTaskDefinition> tasks = new List<MissionTaskDefinition>();

    public string MissionID => missionID;
    public string Title => title;
    public string Description => description;
    public IReadOnlyList<MissionTaskDefinition> Tasks => tasks;

    /// <summary>
    /// Create runtime MissionTask instances from the definitions in this mission.
    /// Typically called by your mission factory/manager.
    /// </summary>
    public List<MissionTask> CreateRuntimeTasks()
    {
        var result = new List<MissionTask>(tasks.Count);
        // Optional: enforce ordering by Step so tasks run in sequence
        var ordered = new List<MissionTaskDefinition>(tasks);
        ordered.Sort((a, b) => a.Step.CompareTo(b.Step));

        foreach (var def in ordered)
        {
            if (def == null) continue;
            var runtimeTask = def.CreateRuntimeTask();
            if (runtimeTask != null)
            {
                result.Add(runtimeTask);
            }
        }
        return result;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Keep the list tidy (no nulls, no duplicates)
        tasks.RemoveAll(t => t == null);
        for (int i = tasks.Count - 1; i >= 0; i--)
        {
            for (int j = i - 1; j >= 0; j--)
            {
                if (tasks[i] == tasks[j])
                {
                    tasks.RemoveAt(i);
                    break;
                }
            }
        }
    }
#endif
}
