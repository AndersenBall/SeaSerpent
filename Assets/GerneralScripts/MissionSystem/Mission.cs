using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class Mission 
{
    public string MissionID { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsCompleted => Tasks.All(t => t.IsCompleted);
    public List<MissionTask> Tasks { get; private set; }
    public int CurrentStep { get; private set; } = 0;

    public Mission(string missionID, string title, string description, List<MissionTask> tasks)
    {
        MissionID = missionID;
        Title = title;
        Description = description;
        Tasks = tasks;

        foreach (var task in Tasks)
        {
            if (task.Step != CurrentStep){
                task.isActive = false;
            }
            else {
                task.isActive = true;
            }
            task.OnTaskCompleted += CheckProgress;
        }
    }

    public void Initialize()
    {
        foreach (var task in Tasks)
        {
            task.Initialize();
            task.isActive = task.Step == CurrentStep;
            task.OnTaskCompleted += CheckProgress;
        }
    }


    private void CheckProgress(MissionTask completedTask)
    {
        // If the task's step matches the current step, check if we can move to the next step
        if (Tasks.Where(t => t.Step == CurrentStep).All(t => t.IsCompleted))
        {
            foreach (var task in Tasks.Where(t => t.Step == CurrentStep))
            {
                task.isActive = false;
            }
            CurrentStep++; // Move to the next step if all tasks in the current step are complete
            foreach (var task in Tasks.Where(t => t.Step == CurrentStep))
            {
                task.isActive = true;
            }
        }

        // Notify the MissionSystem to update mission status if all tasks are complete
        if (IsCompleted)
        {
            Debug.Log("Completed a mission inner:" + this);
            MissionSystem.Instance?.CompleteMission(this);
        }
    }

    public void Cleanup()
    {
        foreach (var task in Tasks)
        {
            task.Cleanup();
            task.OnTaskCompleted -= CheckProgress; // Unsubscribe to avoid memory leaks
        }
    }

    public override string ToString()
    {
        return $"Mission ID: {MissionID}\n" +
               $"Title: {Title}\n" +
               $"Description: {Description}\n" +
               $"Current Step: {CurrentStep}\n" +
               $"Is Completed: {IsCompleted}\n" +
               $"Tasks:\n" +
               string.Join("\n", Tasks.Select(t =>
                   $"  - Task: {t.TaskName}, Step: {t.Step}, Is Active: {t.isActive}, Is Completed: {t.IsCompleted}"));
    }
}
