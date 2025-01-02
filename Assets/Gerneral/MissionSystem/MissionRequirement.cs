using System;

[Serializable]
public class MissionRequirement
{
    public MissionType Type;
    public int RequiredAmount;
    public string TargetID;
    public int CompletionIndex; // Determines the group of requirements that must be completed together
    public string RequirementDescription; // Description for UI display

    public Func<object, bool> Validator; // Custom validator for this requirement

    public MissionRequirement(MissionType type, int requiredAmount, string targetID, int completionIndex, string requirementDescription, Func<object, bool> validator)
    {
        Type = type;
        RequiredAmount = requiredAmount;
        TargetID = targetID;
        CompletionIndex = completionIndex;
        RequirementDescription = requirementDescription;
        Validator = validator;
    }

    public bool Validate(object data)
    {
        return Validator != null && Validator(data);
    }
}