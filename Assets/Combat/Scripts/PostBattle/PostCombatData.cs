using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapMode.Scripts.PostBattle
{
    [Serializable]
    public sealed class PostCombatData
    {
        public Fleet EnemyFleet { get; }
        public IReadOnlyList<Boat> CapturableBoats { get; }
        public IReadOnlyDictionary<string, int> AvailableLoot { get; }
        public int GoldReward { get; }

        public PostCombatData(
            Fleet enemyFleet,
            IEnumerable<Boat> capturableBoats,
            IDictionary<string, int> availableLoot,
            int goldReward)
        {
            EnemyFleet = enemyFleet;
            CapturableBoats = (capturableBoats ?? Enumerable.Empty<Boat>()).ToList();
            AvailableLoot = new Dictionary<string, int>(availableLoot ?? new Dictionary<string, int>());
            GoldReward = Mathf.Max(0, goldReward);
        }
    }

    public sealed class PostCombatSelection
    {
        public IReadOnlyDictionary<string, int> SelectedLoot { get; }
        public IReadOnlyCollection<string> SelectedCapturedBoatNames { get; }

        public PostCombatSelection(
            IDictionary<string, int> selectedLoot,
            IEnumerable<string> selectedCapturedBoatNames)
        {
            SelectedLoot = new Dictionary<string, int>(selectedLoot ?? new Dictionary<string, int>());
            SelectedCapturedBoatNames = new HashSet<string>(selectedCapturedBoatNames ?? Enumerable.Empty<string>());
        }
    }
}
