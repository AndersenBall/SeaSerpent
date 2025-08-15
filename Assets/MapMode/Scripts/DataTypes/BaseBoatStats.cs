using System.Collections.Generic;

public static class BoatStatsDatabase
{
    public static readonly Dictionary<BoatType, BoatStats> BaseStats = new Dictionary<BoatType, BoatStats>
    {
        { BoatType.ManOfWar, new BoatStats(15f, 0.35f, 1200, 400, 70,2000000) },
        { BoatType.Frigate, new BoatStats(20f, 0.65f, 200, 50, 10, 100000) },
        { BoatType.TradeShip, new BoatStats(15f, 0.7f, 50, 100, 6, 75000) }
    };
}
