using UnityEngine;

namespace MapMode.Scripts.DataTypes.boatComponents.Hulls
{
    namespace MapMode.Scripts.DataTypes.boatComponents.Hulls
    {
        public class HullStats
        {
            private int BaseSpeed { get; set; }
            private int BaseManuvrability { get; set; }
            private int BaseDurability { get;  set; } 
            private int BaseStorage { get; set; }

            private int level{ get; set; } = 0;
            private int UpgradeSpeedAmount { get; set; } = 0;
            private int UpgradeManuvrabilityAmount { get; set; } = 0;
            private int UpgradeDurabilityAmount { get; set; } = 0;
            private int UpgradeStorageAmount { get; set; } = 0;

            public HullStats(int baseSpeed, int baseManuvrability, int baseDurability, int baseStorage)
            {
                BaseSpeed = baseSpeed;
                BaseManuvrability = baseManuvrability;
                BaseDurability = baseDurability;
                BaseStorage = baseStorage;
            }

            // Computed properties applying the modifiers
            public int GetTotalSpeedModifier() => BaseSpeed + UpgradeSpeedAmount * level;
            public int GetTotalManuvrabilityModifier() => BaseManuvrability + UpgradeManuvrabilityAmount * level;
            public int GetTotalDurabilityModifier() => BaseDurability + UpgradeDurabilityAmount * level;
            public int GetTotalStorageModifier() => BaseStorage + UpgradeStorageAmount * level;

            public bool Upgrade()
            {
                if (level >= 3){
                    return false;
                }
                level++;
                return true;
            }

        }
    }
}