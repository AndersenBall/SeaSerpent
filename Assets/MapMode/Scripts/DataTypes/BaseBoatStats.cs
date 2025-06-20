using System.Collections.Generic;

public static class BoatStatsDatabase
{
    public static readonly Dictionary<BoatType, BoatStats> BaseStats = new Dictionary<BoatType, BoatStats>
    {
        { BoatType.ManOfWar, new BoatStats(14f, 0.3f, 1200, 1000, 70,2000000) },
        { BoatType.Frigate, new BoatStats(16f, 0.55f, 100, 50, 10, 100000) },
        { BoatType.TradeShip, new BoatStats(12f, 0.5f, 50, 100, 6, 75000) }
    };
}
