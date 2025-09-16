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

        // enemyFleet: pre-combat fleet (so its inventory is intact)
        // enemyBoatsToRemove: the boats that were destroyed/sunk in the battle
        // sunkFraction: fraction of loot recoverable from sunk ships (e.g., 0.30f)
        // aliveLootFraction: fraction of loot recoverable from alie ships
        // goldReward: total gold computed from boats (30% for sunk, 100% for the rest)
        public static Dictionary<string, int> ComputeAvailableLoot(
            Fleet enemyFleet,
            IList<Boat> enemyBoatsToRemove,
            float sunkFraction,
            float aliveLootFraction,
            out int goldReward)
        {
            if (enemyFleet == null) throw new ArgumentNullException(nameof(enemyFleet));

            sunkFraction = Mathf.Clamp01(sunkFraction);

            var allBoats = enemyFleet.GetBoats() ?? new List<Boat>();
            int totalBoats = allBoats.Count;
            int removedBoats = enemyBoatsToRemove != null ? Mathf.Clamp(enemyBoatsToRemove.Count, 0, totalBoats) : 0;
            int aliveBoats = totalBoats - removedBoats;

            float aliveRatio = totalBoats > 0 ? (float)aliveBoats / totalBoats : 0f;
            float lootFraction = Mathf.Clamp01(sunkFraction * (1f - aliveRatio) +  aliveRatio * aliveLootFraction);
            
            var removedSet = enemyBoatsToRemove != null
                ? new HashSet<Boat>(enemyBoatsToRemove)
                : new HashSet<Boat>();

            long goldAccumulator = 0;
            for (int i = 0; i < allBoats.Count; i++)
            {
                var boat = allBoats[i];
                if (!BoatTypeBaseGold.TryGetValue(boat.boatType, out int baseGold)) continue;

                bool isSunk = removedSet.Contains(boat);
                float goldFrac = isSunk ? sunkFraction : aliveLootFraction;
                goldAccumulator += Mathf.RoundToInt(baseGold * goldFrac);
            }

            // Clamp to int range
            goldReward = (int)Mathf.Max(0, (int)goldAccumulator);

            // Pull pre-combat inventory and compute item loot by sampling
            var (ids, counts) = enemyFleet.GetInventory();
            var result = new Dictionary<string, int>(ids.Length);

            for (int i = 0; i < ids.Length; i++)
            {
                string id = ids[i];
                int qty = counts[i];
                if (qty <= 0) continue;

                int kept = 0;
                for (int n = 0; n < qty; n++)
                {
                    if (UnityEngine.Random.value < lootFraction) kept++;
                }

                if (kept > 0)
                {
                    if (result.TryGetValue(id, out int cur))
                        result[id] = cur + kept;
                    else
                        result[id] = kept;
                }
            }

            return result;
        }
    }

}