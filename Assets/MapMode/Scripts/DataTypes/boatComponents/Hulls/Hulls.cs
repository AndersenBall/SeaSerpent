using UnityEngine;

namespace MapMode.Scripts.DataTypes.boatComponents.Hulls
{
    [System.Serializable]
    public class Hull : IBoatComponent
    {
        public string Name { get; private set; }
        public float SpeedMultiplier { get; private set; }
        public float DurabilityMultiplier { get; private set; }

        public Hull(string name, float speedMultiplier, float durabilityMultiplier)
        {
            Name = name;
            SpeedMultiplier = speedMultiplier;
            DurabilityMultiplier = durabilityMultiplier;
        }

        public void ApplyEffect(BoatStats stats)
        {
            stats.speed *= SpeedMultiplier;
            stats.health = Mathf.RoundToInt(stats.health * DurabilityMultiplier);
        }
    }
}