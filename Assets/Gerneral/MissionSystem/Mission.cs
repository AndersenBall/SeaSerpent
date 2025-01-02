using System.Collections.Generic;
using System;

[Serializable]
public class Mission
{
    public string MissionID;
    public string Title;
    public string Description;
    public bool IsCompleted;
    public List<MissionRequirement> Requirements;
    public int CurrentCompletionIndex;

    public Mission(string missionID, string title, string description, List<MissionRequirement> requirements)
    {
        MissionID = missionID;
        Title = title;
        Description = description;
        Requirements = requirements;
        IsCompleted = false;
        CurrentCompletionIndex = 0;
    }

    public bool CompleteRequirement(string targetID, object data)
    {
        bool allInCurrentIndexCompleted = true;
        bool anyRequirementCompleted = false;

        foreach (var requirement in Requirements)
        {
            if (requirement.CompletionIndex == CurrentCompletionIndex && requirement.TargetID == targetID)
            {
                if (requirement.Validate(data))
                {
                    anyRequirementCompleted = true;
                }
                else
                {
                    allInCurrentIndexCompleted = false;
                }
            }
            else if (requirement.CompletionIndex == CurrentCompletionIndex)
            {
                allInCurrentIndexCompleted = false;
            }
        }

        // If all requirements in the current index are completed, move to the next index
        if (allInCurrentIndexCompleted)
        {
            CurrentCompletionIndex++;

            // Check if all requirements have been completed
            if (CurrentCompletionIndex > GetMaxCompletionIndex())
            {
                IsCompleted = true;
            }
            return true;
        }

        return anyRequirementCompleted;
    }

    private int GetMaxCompletionIndex()
    {
        int maxIndex = 0;
        foreach (var requirement in Requirements)
        {
            if (requirement.CompletionIndex > maxIndex)
            {
                maxIndex = requirement.CompletionIndex;
            }
        }
        return maxIndex;
    }
}
