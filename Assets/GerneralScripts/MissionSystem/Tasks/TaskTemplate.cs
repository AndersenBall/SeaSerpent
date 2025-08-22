using UnityEngine;

public abstract class TaskTemplate : ScriptableObject
{
    [SerializeField] protected string taskName;
    [SerializeField] protected int step;

    public string TaskName => taskName;
    public int Step => step;
    
    public abstract TaskInstance CreateRuntimeTask( );
    
}
