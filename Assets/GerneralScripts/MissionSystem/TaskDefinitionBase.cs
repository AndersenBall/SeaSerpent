using UnityEngine;

public abstract class TaskDefinition : ScriptableObject
{
    [SerializeField] private string taskName;
    [SerializeField] private int step;

    public string TaskName => taskName;
    public int Step => step;

    // Factory hook to create a concrete MissionTask for runtime
    public abstract MissionTask CreateRuntimeTask(TownContext context);
}