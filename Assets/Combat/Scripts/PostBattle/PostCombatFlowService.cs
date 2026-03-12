using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapMode.Scripts.PostBattle
{
    public static class PostCombatFlowService
    {
        public static event Action<PostCombatData> PostCombatReady;

        public static PostCombatData CurrentPostCombatData { get; private set; }
        public static bool IsPostCombatActive => CurrentPostCombatData != null;

        public static void BeginPostCombat(PostCombatData postCombatData)
        {
            CurrentPostCombatData = postCombatData;
            PostCombatReady?.Invoke(postCombatData);

            // If no UI has subscribed yet, continue with a safe default so combat does not soft-lock.
            if (PostCombatReady == null)
            {
                ResolvePostCombat(new PostCombatSelection(
                    selectedLoot: postCombatData.AvailableLoot.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
                    selectedCapturedBoatNames: Array.Empty<string>()));
            }
        }

        public static void ResolvePostCombat(PostCombatSelection selection)
        {
            if (!IsPostCombatActive)
            {
                Debug.LogWarning("ResolvePostCombat called with no active post-combat state.");
                return;
            }

            var data = CurrentPostCombatData;

            ApplySelectedLoot(selection.SelectedLoot, data.AvailableLoot);
            ApplySelectedCapturedShips(selection.SelectedCapturedBoatNames, data.CapturableBoats, data.EnemyFleet);
            PlayerStateService.AddMoney(data.GoldReward);

            CurrentPostCombatData = null;
            SceneTransfer.TransferToMap();
        }

        private static void ApplySelectedLoot(
            IReadOnlyDictionary<string, int> selectedLoot,
            IReadOnlyDictionary<string, int> availableLoot)
        {
            if (PlayerStateService.PlayerFleet == null)
            {
                return;
            }

            foreach (var kvp in selectedLoot)
            {
                if (!availableLoot.TryGetValue(kvp.Key, out var availableCount))
                {
                    continue;
                }

                var amountToAdd = Mathf.Clamp(kvp.Value, 0, availableCount);
                if (amountToAdd <= 0)
                {
                    continue;
                }

                AddCargoToPlayerFleet(kvp.Key, amountToAdd);
            }
        }

        private static void AddCargoToPlayerFleet(string itemId, int amount)
        {
            var boats = PlayerStateService.PlayerFleet.GetBoats();
            if (boats == null || boats.Count == 0)
            {
                return;
            }

            int remaining = amount;
            foreach (var boat in boats)
            {
                if (remaining <= 0)
                {
                    break;
                }

                int added = boat.AddCargo(itemId, remaining);
                remaining -= added;
            }
        }

        private static void ApplySelectedCapturedShips(
            IReadOnlyCollection<string> selectedCapturedBoatNames,
            IReadOnlyList<Boat> capturableBoats,
            Fleet enemyFleet)
        {
            if (PlayerStateService.PlayerFleet == null || enemyFleet == null)
            {
                return;
            }

            var capturableByName = capturableBoats.ToDictionary(boat => boat.boatName, boat => boat);
            foreach (var selectedName in selectedCapturedBoatNames)
            {
                if (!capturableByName.TryGetValue(selectedName, out var selectedBoat))
                {
                    continue;
                }

                if (!enemyFleet.HasBoatWithName(selectedName))
                {
                    continue;
                }

                if (PlayerStateService.PlayerFleet.HasBoatWithName(selectedName))
                {
                    continue;
                }

                enemyFleet.RemoveBoat(selectedName);
                PlayerStateService.PlayerFleet.AddBoat(selectedBoat);
            }
        }
    }
}
