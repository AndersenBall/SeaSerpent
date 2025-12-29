using System.Collections.Generic;

namespace GerneralScripts.BattleManagement
{
    public sealed class SideResult
    {
        public Dictionary<string, int> HpByBoatId { get; } = new();
        public HashSet<string> DestroyedBoatIds { get; } = new();
    }

    public sealed class BattleResult
    {
        public string SessionId { get; }
        public BattleSide Winner { get; }

        public SideResult A { get; } = new();
        public SideResult B { get; } = new();

        // Optional: metadata you’ll want for UX / respawn / rewards
        public bool PlayerDefeated { get; set; }
        public BattleResult(string sessionId, BattleSide winner)
        {
            SessionId = sessionId;
            Winner = winner;
        }
    }

}