using System.Collections.Generic;

public static class SailorStatsDatabase
{
    public static readonly Dictionary<SailorType, SailorStats> BaseStats = new Dictionary<SailorType, SailorStats>
    {
        { SailorType.Captain, new SailorStats(15, new List<string> { "Sword" }, 6, 4, 8, 10, 8, 10, 12, 500) },
        { SailorType.Gunner, new SailorStats(10, new List<string> { "Pistol" }, 5, 8, 6, 7, 5, 6, 5, 300) },
        { SailorType.Navigator, new SailorStats(8, new List<string> { "Dagger" }, 4, 3, 7, 5, 10, 5, 6, 250) },
        { SailorType.Carpenter, new SailorStats(12, new List<string> { "Hammer" }, 4, 5, 5, 6, 4, 8, 10, 350) },
        { SailorType.Reaver, new SailorStats(20, new List<string> { "Axe", "Sword" }, 7, 3, 5, 8, 10, 2, 3, 400) }
    };
}