using System.Collections.Generic;
using System.Linq;

namespace GerneralScripts.BattleManagement
{
    public static class AutoResolver
{
    public static BattleResult Resolve(BattleSession session)
    {
        var aFleet = session.SideA.Fleet;
        var bFleet = session.SideB.Fleet;

        // Clone boat state so we don't mutate live data mid-sim
        var aSim = aFleet.GetBoats().Select(CloneForSim).ToList();
        var bSim = bFleet.GetBoats().Select(CloneForSim).ToList();

        var rng = new System.Random();

        while (aSim.Count > 0 && bSim.Count > 0)
        {
            SimulateRound(aSim, bSim, rng);
            SimulateRound(bSim, aSim, rng);

            aSim.RemoveAll(b => b.HP <= 0);
            bSim.RemoveAll(b => b.HP <= 0);
        }

        var winner = aSim.Count > 0 ? BattleSide.A : BattleSide.B;
        var result = new BattleResult(session.SessionId, winner);

        BuildSideResult(
            original: aFleet.GetBoats(),
            sim: aSim,
            resultSide: result.A
        );

        BuildSideResult(
            original: bFleet.GetBoats(),
            sim: bSim,
            resultSide: result.B
        );

        // Player aftermath info
        if (session.HasPlayer)
        {
            var playerSide = session.PlayerSide;
            result.PlayerDefeated =
                (playerSide == BattleSide.A && winner == BattleSide.B) ||
                (playerSide == BattleSide.B && winner == BattleSide.A);
        }

        return result;
    }

    // -----------------------------

    private static void SimulateRound(
        List<SimBoat> attackers,
        List<SimBoat> defenders,
        System.Random rng)
    {
        foreach (var attacker in attackers)
        {
            if (defenders.Count == 0) break;

            var target = defenders[rng.Next(defenders.Count)];
            var damage = (int) (attacker.Attack * (0.75f + (float)rng.NextDouble() * 0.5f));

            target.HP -= damage;
        }
    }

    private static void BuildSideResult(
        List<Boat> original,
        List<SimBoat> sim,
        SideResult resultSide)
    {
        var simById = sim.ToDictionary(b => b.BoatId);

        foreach (var boat in original)
        {
            if (!simById.TryGetValue(boat.BoatId, out var simBoat))
            {
                resultSide.DestroyedBoatIds.Add(boat.BoatId);
            }
            else
            {
                resultSide.HpByBoatId[boat.BoatId] = simBoat.HP;
            }
        }
    }

    private static SimBoat CloneForSim(Boat boat)
    {
        return new SimBoat
        {
            BoatId = boat.BoatId,
            HP = boat.currentBoatHealth,
            Attack = boat.maxSailorCount //TODO make power calc for a boat
        };
    }

    // -----------------------------
    // Pure simulation model
    private sealed class SimBoat
    {
        public string BoatId;
        public int HP;
        public float Attack;
    }
}

}