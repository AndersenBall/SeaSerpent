using System.Collections.Generic;

public static class BoatStatsDatabase
{
    public static readonly Dictionary<BoatType, BoatStats> BaseStats = new Dictionary<BoatType, BoatStats>
    {
        { BoatType.ManOfWar, new BoatStats(12f, 0.35f, 1200, 400, 70,2000000) },
        { BoatType.Frigate, new BoatStats(15f, 0.65f, 200, 50, 10, 100000) },
        { BoatType.TradeShip, new BoatStats(12f, 0.7f, 50, 100, 6, 75000) },
        { BoatType.Sloop, new BoatStats(15f, 1f, 40, 10, 4, 10000) }
    };
    
    public static readonly Dictionary<BoatType, float> BaseTypePower = new Dictionary<BoatType, float>
    {
        { BoatType.ManOfWar,   10.0f },
        { BoatType.Frigate, 3f },
        { BoatType.TradeShip, 1f },
        { BoatType.Sloop , 1f}
    };
    
    public static float GetBaseTypePower(BoatType t) =>
        BaseTypePower.TryGetValue(t, out var p) ? p : 1.0f;

    
    
}
