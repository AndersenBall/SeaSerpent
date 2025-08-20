using UnityEngine;

public abstract class TaskTemplate : ScriptableObject
{
    [SerializeField] private string taskName;
    [SerializeField] private int step;

    public string TaskName => taskName;
    public int Step => step;
    
    public abstract MissionTask CreateRuntimeTask( );
}
