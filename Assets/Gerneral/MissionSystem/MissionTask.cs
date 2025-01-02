using System;

[Serializable]
public abstract class MissionTask
{
    public string TaskName { get; protected set; }
    public int Step { get; private set; }
    public bool IsCompleted { get; protected set; } = false;

    public bool isActive { get; set; } = false;

    public event Action<MissionTask> OnTaskCompleted;

    protected MissionTask(string taskName, int step)
    {
        TaskName = taskName;
        Step = step;
    }

    public abstract void CheckProgress();

    protected void CompleteTask()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            OnTaskCompleted?.Invoke(this); // Notify listeners
        }
    }
}
