using System.Linq;
using UnityEngine;

namespace MapMode.Scripts
{
    public class BattlePredicter
    {
        public static float GetFleetPower(Fleet fleet)
        {
            if (fleet == null) return 0f;
            float total = 0f;
            
            foreach (var boat in fleet.GetBoats())
            {
                if (boat == null) continue;

                float basePower = BoatStatsDatabase.GetBaseTypePower(boat.boatType);

                int maxHp = Mathf.Max(1, boat.maxBoatHealth);
                float hpPct = Mathf.Clamp01((float)boat.currentBoatHealth / maxHp);

                total += basePower * hpPct;
            }
            return total;
        }

        public static float CalculateWinner(Fleet fleet1, Fleet fleet2)
        {
            float power1 = GetFleetPower(fleet1);
            float power2 = GetFleetPower(fleet2);
            
            power1 *= UnityEngine.Random.Range(0.9f, 1.1f);
            power2 *= UnityEngine.Random.Range(0.9f, 1.1f);

            if (Mathf.Approximately(power1, power2))
                return 0f;

            float diff = power1 - power2;
            float sum = power1 + power2;

            // Normalize to -1..1
            return diff / Mathf.Max(sum, 0.0001f);
        }
        
        public static void ApplyBattleDamage(Fleet fleet1, Fleet fleet2)
        {
            // Outcome from -1 to 1
            float outcome = CalculateWinner(fleet1, fleet2);

            // Determine winner & loser
            Fleet winner = outcome >= 0 ? fleet1 : fleet2;
            Fleet loser  = outcome >= 0 ? fleet2 : fleet1;

            float margin = Mathf.Abs(outcome);

            // Map margin -> damage fraction
            // At 0 (tie/near win) → 0.9 (90% damage)
            // At 0.1 → ~0.8 (80% damage)
            // At 1.0 → 0.3 (20% damage)
            float damageFraction = Mathf.Lerp(0.9f, 0.3f, Mathf.Clamp01(margin));

            // Total HP pool of winner
            var ships = winner.GetBoats();
            int totalHp = ships.Sum(b => b.currentBoatHealth);

            if (totalHp <= 0) return;

            int damagePool = Mathf.RoundToInt(totalHp * damageFraction);

            // Randomly distribute damage among ships
            while (damagePool > 0 && ships.Count > 0)
            {
                var boat = ships[UnityEngine.Random.Range(0, ships.Count)];
                if (boat.currentBoatHealth <= 0) continue;

                int damage = UnityEngine.Random.Range(1, Mathf.Min(boat.currentBoatHealth, damagePool) + 1);
                boat.currentBoatHealth = Mathf.Max(0, boat.currentBoatHealth - damage);
                damagePool -= damage;
            }

            // Loser fleet is destroyed (optional)
            foreach (var boat in loser.GetBoats())
            {
                boat.currentBoatHealth = 0;
            }
        }
        public static void ApplyBattleDamage(Fleet winner, float damageFraction)
        {
            var ships = winner.GetBoats();
            if (ships == null || ships.Count == 0) return;

            int totalHp = ships.Sum(b => b.currentBoatHealth);
            if (totalHp <= 0) return;

            int damagePool = Mathf.RoundToInt(totalHp * damageFraction);
            int n = ships.Count;

            // --- Step 1: start equal weights ---
            float[] weights = new float[n];
            for (int i = 0; i < n; i++)
                weights[i] = 1f / n;

            // --- Step 2: random shuffling of weights ---
            int shuffleCount = n * 3; // you can tweak how "uneven" it gets
            for (int i = 0; i < shuffleCount; i++)
            {
                int from = UnityEngine.Random.Range(0, n);
                int to = UnityEngine.Random.Range(0, n);
                if (from == to) continue;

                // Pick a small random amount to transfer
                float delta = UnityEngine.Random.Range(0f, weights[from] * 0.5f); 
                weights[from] -= delta;
                weights[to] += delta;
            }

            // --- Step 3: assign damage based on weights ---
            for (int i = 0; i < n; i++)
            {
                var boat = ships[i];
                if (boat.currentBoatHealth <= 0) continue;

                int dmg = Mathf.RoundToInt(damagePool * weights[i]);
                boat.currentBoatHealth = Mathf.Max(0, boat.currentBoatHealth - dmg);
            }
        }
    }
}