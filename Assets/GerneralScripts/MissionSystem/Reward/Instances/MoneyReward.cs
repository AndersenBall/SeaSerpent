using System;

namespace GerneralScripts.MissionSystem.Reward
{
    [Serializable]
    public sealed class MoneyReward : IMissionReward
    {
        public float Amount;

        public MoneyReward(float amount)
        {
            Amount = amount;
        }

        public void Apply()
        {
            if (Amount <= 0f) return;
            
            PlayerGlobal.AddMoney(Amount);
        }

        public string GetDescription()
        {
            var scaled = Amount ;
            return $"+{scaled:0} money";
        }
    }

}