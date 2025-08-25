namespace GerneralScripts.MissionSystem.Reward
{
    
    public interface IMissionReward
    {
        void Apply();
        
        string GetDescription();
    }
}