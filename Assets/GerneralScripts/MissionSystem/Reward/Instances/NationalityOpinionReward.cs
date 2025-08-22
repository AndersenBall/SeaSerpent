using System;
using GerneralScripts.MissionSystem.Reward;

[Serializable]
public sealed class NationalityOpinionReward : IMissionReward
{
    public Nation Nation;
    public int Delta;

    public NationalityOpinionReward(Nation nation, int delta)
    {
        Nation = nation;
        Delta = delta;
    }

    public void Apply()
    {
        NationalityOpinionSystem.Instance.ModifyOpinion(Nation.Spain, Delta);
    }

    public string GetDescription()
    {
        return $"{Nation}: opinion {(Delta <= 0 ? "-" : "")}{Delta:0}";
    }
}
