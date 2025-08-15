namespace MapMode.Scripts.DataTypes.boatComponents.Sails
{
    [System.Serializable]
    public class Sails : IBoatComponent
    {
        public SailType Type { get; private set; }
        public SailStats SailStats { get; private set; }

        public Sails(SailType type)
        {
            Type = type;
            if (SailStatsDatabase.BaseStats.TryGetValue(type, out SailStats stats))
            {
                SailStats = stats;
            }
        }

        public void ApplyEffect(BoatStats stats)
        {
            if (SailStats == null) return;
            stats.speed += SailStats.speedMultiplier;
            stats.turnSpeed += SailStats.turnSpeedMultiplier;
            stats.maxSailHealth += SailStats.sailHealth;
        }

    }
}