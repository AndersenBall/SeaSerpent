using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GerneralScripts.BattleManagement
{
    public static class BattleResultFactory
    {
        public static BattleResult BuildFromPlayableCombat(
            BattleSession session,
            IReadOnlyList<Boat> initialPlayerBoats,
            IReadOnlyList<Boat> currentPlayerBoats,
            IReadOnlyList<Boat> initialEnemyBoats,
            IReadOnlyList<Boat> currentEnemyBoats,
            bool playerDefeated)
        {
            if (session == null)
            {
                return null;
            }

            var enemySurvivors = currentEnemyBoats?.Count ?? 0;
            var playerWon = enemySurvivors == 0;
            var winner = playerWon ? session.PlayerSide : Opposite(session.PlayerSide);

            var result = new BattleResult(session.SessionId, winner)
            {
                PlayerDefeated = playerDefeated
            };

            if (session.PlayerSide == BattleSide.A)
            {
                BuildSideResult(initialPlayerBoats, currentPlayerBoats, result.A);
                BuildSideResult(initialEnemyBoats, currentEnemyBoats, result.B);
            }
            else
            {
                BuildSideResult(initialEnemyBoats, currentEnemyBoats, result.A);
                BuildSideResult(initialPlayerBoats, currentPlayerBoats, result.B);
            }

            return result;
        }

        private static void BuildSideResult(
            IReadOnlyList<Boat> initialBoats,
            IReadOnlyList<Boat> currentBoats,
            SideResult side)
        {
            var currentById = (currentBoats ?? new List<Boat>())
                .ToDictionary(GetBoatKey, boat => boat);

            foreach (var initialBoat in initialBoats ?? new List<Boat>())
            {
                var boatId = GetBoatKey(initialBoat);
                if (currentById.TryGetValue(boatId, out var currentBoat))
                {
                    side.HpByBoatId[boatId] = Mathf.Max(0, currentBoat.currentBoatHealth);
                }
                else
                {
                    side.DestroyedBoatIds.Add(boatId);
                }
            }
        }

        private static string GetBoatKey(Boat boat)
        {
            if (!string.IsNullOrWhiteSpace(boat.BoatId))
            {
                return boat.BoatId;
            }

            return boat.boatName;
        }

        private static BattleSide Opposite(BattleSide side)
        {
            return side == BattleSide.A ? BattleSide.B : BattleSide.A;
        }
    }
}
