using UnityEngine;

namespace GerneralScripts.BattleManagement
{
    public static class BattleResultApplier
    {
        public static void Apply(BattleSession session, BattleResult result)
        {
            ApplySide(session.SideA.Fleet, result.A);
            ApplySide(session.SideB.Fleet, result.B);

            if (session.HasPlayer)
                ApplyPlayerAftermath(session, result);

            GameEvents.SaveGame();
        }

        private static void ApplySide(Fleet fleet, SideResult side)
        {
            ApplyFleet(fleet, side);
            UpdateWorldFleetObject(fleet);
        }

        private static void ApplyFleet(Fleet fleet, SideResult side)
        {
            var boats = fleet.GetBoats();
            boats.RemoveAll(b => side.DestroyedBoatIds.Contains(b.BoatId));

            foreach (var b in boats)
                if (side.HpByBoatId.TryGetValue(b.BoatId, out int hp))
                    b.currentBoatHealth = hp;

            fleet.SetBoats(boats);
        }

        private static void UpdateWorldFleetObject(Fleet fleet)
        {
            // You can improve this later (WorldContext instead of Find)
            var boatsRoot = GameObject.Find("Boats");
            if (!boatsRoot) return;

            foreach (var ctrl in boatsRoot.GetComponentsInChildren<FleetMapController>(true))
            {
                if (ctrl.GetFleet().FleetID != fleet.FleetID) continue;

                ctrl.SetFleet(fleet);
                if (fleet.getNumberBoats() == 0)
                    UnityEngine.Object.Destroy(ctrl.gameObject);
                break;
            }
        }
        
        private static void ApplyPlayerAftermath(BattleSession session, BattleResult result)
        {
            // “Only difference is player might have to respawn”
            // Put respawn logic here so battle logic remains identical for AI/Player.
            if (result.PlayerDefeated)
            {
                // Example hooks:
                // PlayerRespawnService.RespawnAtNearestPort();
                // InventoryService.DropLoot(...);
            }
        }
    }
}