using System;
using GerneralScripts.MissionSystem.Reward;

[Serializable]
public sealed class NationalityOpinionReward : IMissionReward
{
    public Nation Nation;
    public float Delta;

    public NationalityOpinionReward(Nation nation, float delta)
    {
        Nation = nation;
        Delta = delta;
    }

    public void Apply()
    {
       
    }

    public string GetDescription()
    {
        return $"{Nation}: opinion {(Delta <= 0 ? "-" : "")}{Delta:0}";
    }
}
