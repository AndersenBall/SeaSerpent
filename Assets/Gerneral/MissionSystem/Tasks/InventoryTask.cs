using System;
using System.Collections.Generic;

[Serializable]
public class InventoryTask : MissionTask
{
    public Dictionary<string, int> RequiredItems { get; private set; } // Items required to complete the task
    public Dictionary<string, int> CurrentItems { get; private set; } 

    public InventoryTask(string taskName, int step, Dictionary<string, int> itemsToCollect)
        : base(taskName, step)
    {
        RequiredItems = itemsToCollect;
        CurrentItems = new Dictionary<string, int>();
    }

    public void OnItemCollected(string itemName, int quantity)
    {
        // Check if the task is active
        if (!isActive || IsCompleted) return;

        if (RequiredItems.ContainsKey(itemName))
        {
            if (!CurrentItems.ContainsKey(itemName))
                CurrentItems[itemName] = 0;

            CurrentItems[itemName] += quantity;
            CheckProgress();
        }
    }

    public override void CheckProgress()
    {
        foreach (var item in RequiredItems)
        {
            if (!CurrentItems.ContainsKey(item.Key) || CurrentItems[item.Key] < item.Value)
                return;
        }

        CompleteTask();
    }
}
