using MapMode.Scripts.DataTypes.boatComponents.Hulls.MapMode.Scripts.DataTypes.boatComponents.Hulls;

namespace MapMode.Scripts.DataTypes.boatComponents.Hulls
{
    [System.Serializable]
    public class Hull : IBoatComponent
    {
        public HullType Type { get; private set; }
        public HullStats HullStats { get; private set; }
        
        public Hull(HullType type)
        {
            Type = type;
            if (HullStatsDatabase.BaseStats.TryGetValue(type, out HullStats stats)){
                HullStats = stats;
            }
        }

        public void ApplyEffect(BoatStats stats)
        {
            if (HullStats == null) return;
            stats.speed += HullStats.GetTotalSpeedModifier();
            stats.turnSpeed += HullStats.GetTotalManuvrabilityModifier();
            stats.maxHealth += HullStats.GetTotalDurabilityModifier();
            stats.cargoMax += HullStats.GetTotalStorageModifier();

        }
    }
}