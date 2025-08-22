using System;

namespace GerneralScripts.MissionSystem.Reward.Templates
{
    public class MoneyRewardTemplate:RewardTemplate
    {
        private int reward;

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