using UnityEngine;

namespace GerneralScripts.MissionSystem.Reward
{
    public abstract class RewardTemplate : ScriptableObject
    {
        public abstract IMissionReward CreateRuntimeReward( );
    }
}