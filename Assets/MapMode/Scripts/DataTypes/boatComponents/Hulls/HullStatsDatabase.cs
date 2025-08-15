using System.Collections.Generic;
using MapMode.Scripts.DataTypes.boatComponents.Hulls.MapMode.Scripts.DataTypes.boatComponents.Hulls;

namespace MapMode.Scripts.DataTypes.boatComponents.Hulls
{
    public static class HullStatsDatabase
    {
        public static readonly Dictionary<HullType, HullStats> BaseStats = new Dictionary<HullType, HullStats>
        {
            // Speed, Maneuverability, Durability, Storage
            { HullType.LongNarrow, new HullStats(5, 5, 20, 0) },
            { HullType.WideFlat, new HullStats(2, 2, 30, 50) },
            { HullType.Rounded, new HullStats(3, 3, 40, 20) },
            { HullType.DeepV, new HullStats(4, 3, 30, 20) },
            { HullType.ShallowDraft, new HullStats(4, 4, 20, 0) },
            { HullType.Armored, new HullStats(2, 1, 50, 10) },
            { HullType.Hybrid, new HullStats(3, 3, 30, 30) }
        };
    }
}