namespace MapMode.Scripts.DataTypes.boatComponents.Cannons
{
    using System.Collections.Generic;
    
    public static class CannonStatsDatabase
    {
        public static readonly Dictionary<CannonType, CannonStats> BaseStats = new Dictionary<CannonType, CannonStats>
        {
            {
                CannonType.LongGun,
                new CannonStats(24, 150f, 0, -35f, 10f, 25f, 5f, 1.5f, 1000)
            },
            {
                CannonType.Carronade,
                new CannonStats(32, 100f, 20, -15f, 10f, 0f, 3f, 2f, 1200)
            },
            {
                CannonType.Mortar,
                new CannonStats(64, 50f, 30, -80f, -25f, 360f, 15f, 5f, 2000)
            },
            {
                CannonType.FlameThrower,
                new CannonStats(0, 40f, 50, -15f, 15f, 30f, 15f, 1.5f, 1500)
            },
            {
                CannonType.GrappleHook,
                new CannonStats(0, 100f, 10, -30f, 60f, 90f, 20f, 1f, 800)
            }
        };
    }

    
}