using System;

[Serializable]
public abstract class TaskInstance
{
    public string TaskName { get; protected set; }
    public int Step { get; private set; }
    public bool IsCompleted { get; protected set; } = false;

    public bool isActive { get; set; } = false;

    public event Action<TaskInstance> OnTaskCompleted;

    protected TaskInstance(string taskName, int step)
    {
        TaskName = taskName;
        Step = step;
    }

    public abstract void Initialize();

    public abstract void CheckProgress();

    protected void CompleteTask()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            OnTaskCompleted?.Invoke(this); // Notify listeners
        }
    }
    public virtual void Cleanup()
    {
        OnTaskCompleted = null; // Unsubscribe all event listeners
    }
}
