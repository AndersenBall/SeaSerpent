using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapMode.Scripts.PostBattle
{

    public static class LootUtils
    {
        private static readonly Dictionary<BoatType, int> BoatTypeBaseGold = new Dictionary<BoatType, int>
        {
            { BoatType.ManOfWar, 100000 },
            { BoatType.Frigate, 10000 },
            { BoatType.TradeShip, 5000 }
        };

        public static Dictionary<string, int> ComputeAvailableLoot(
            IList<Boat> survivingEnemyBoats,
            IList<Boat> sunkEnemyBoats,
            float sunkLootFraction,
            out int goldReward)
        {
            var availableLoot = new Dictionary<string, int>();
            long goldAccumulator = 0;

            sunkLootFraction = Mathf.Clamp01(sunkLootFraction);
            survivingEnemyBoats ??= new List<Boat>();
            sunkEnemyBoats ??= new List<Boat>();

            foreach (var boat in survivingEnemyBoats)
            {
                goldAccumulator += GetBoatGold(boat, 1f);
                AddBoatSuppliesToLoot(boat, availableLoot, 1f);
            }

            foreach (var boat in sunkEnemyBoats)
            {
                goldAccumulator += GetBoatGold(boat, sunkLootFraction);
                AddBoatSuppliesToLoot(boat, availableLoot, sunkLootFraction);
            }

            goldReward = Mathf.Max(0, (int)goldAccumulator);
            return availableLoot;
        }

        private static int GetBoatGold(Boat boat, float amountFraction)
        {
            if (boat == null || !BoatTypeBaseGold.TryGetValue(boat.boatType, out var baseGold))
            {
                return 0;
            }

            return Mathf.RoundToInt(baseGold * Mathf.Clamp01(amountFraction));
        }

        private static void AddBoatSuppliesToLoot(Boat boat, Dictionary<string, int> loot, float amountFraction)
        {
            if (boat == null)
            {
                return;
            }

            foreach (var itemKvp in boat.getSupplies())
            {
                if (itemKvp.Value <= 0)
                {
                    continue;
                }

                int recovered = RollRecoveredQuantity(itemKvp.Value, amountFraction);
                if (recovered <= 0)
                {
                    continue;
                }

                if (loot.TryGetValue(itemKvp.Key, out var existing))
                {
                    loot[itemKvp.Key] = existing + recovered;
                }
                else
                {
                    loot[itemKvp.Key] = recovered;
                }
            }
        }

        private static int RollRecoveredQuantity(int totalItems, float probability)
        {
            probability = Mathf.Clamp01(probability);
            if (probability >= 1f)
            {
                return totalItems;
            }

            int recovered = 0;
            for (int i = 0; i < totalItems; i++)
            {
                if (UnityEngine.Random.value <= probability)
                {
                    recovered++;
                }
            }

            return recovered;
        }
    }

}
