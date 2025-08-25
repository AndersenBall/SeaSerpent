using System;
using UnityEngine;

namespace GerneralScripts.MissionSystem.Reward.Templates
{
    [CreateAssetMenu(menuName = "Rewards/New Money Reward", fileName = "MoneyRewardTemplate")]
    public class MoneyRewardTemplate:RewardTemplate
    {
        [SerializeField] private int reward = 1000;

        public MoneyRewardTemplate(int reward)
        {
            this.reward = reward;
        }

        public override IMissionReward CreateRuntimeReward()
        {
            return new MoneyReward(reward);
        }
    }
}