using System.Collections.Generic;

namespace MapMode.Scripts.DataTypes.boatComponents.Sails
{
    public class SailStatsDatabase
    {
        public static readonly Dictionary<SailType, SailStats> BaseStats = new Dictionary<SailType, SailStats>
        {
            // Speed, Turn Speed, Sail Health, SweetSpotStrength
            { SailType.Square, new SailStats { speedMultiplier = 1.4f, turnSpeedMultiplier = 0.8f, sailHealth = 80, sweetSpotStrength = 1.2f } },
            { SailType.Lateen, new SailStats { speedMultiplier = 1.1f, turnSpeedMultiplier = 1.4f, sailHealth = 70, sweetSpotStrength = 1.0f } },
            { SailType.Balanced, new SailStats { speedMultiplier = 1.2f, turnSpeedMultiplier = 1.2f, sailHealth = 75, sweetSpotStrength = 1.1f } },
            { SailType.HeavyCanvas, new SailStats { speedMultiplier = 0.9f, turnSpeedMultiplier = 0.9f, sailHealth = 120, sweetSpotStrength = 0.8f } },
            { SailType.FineRigged, new SailStats { speedMultiplier = 1.3f, turnSpeedMultiplier = 1.3f, sailHealth = 60, sweetSpotStrength = 1.5f } }
        };
    }
}