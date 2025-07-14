namespace MapMode.Scripts.DataTypes.boatComponents.Cannons
{
    public class CannonStats
    {
        public int ShotWeight { get; }
        public float ShotPower { get; }
        public int BaseDamage { get; }
        public float MinVerticalAngle { get; }
        public float MaxVerticalAngle { get; }
        public float MaxHorizontalAngle { get; }
        public float TurnSpeed { get; }
        public float Variance { get; }
        public int Cost { get; }

        public CannonStats(
            int shotWeight, 
            float shotPower, 
            int baseDamage, 
            float minVerticalAngle, 
            float maxVerticalAngle, 
            float maxHorizontalAngle, 
            float turnSpeed, 
            float variance, 
            int cost)
        {
            ShotWeight = shotWeight;
            ShotPower = shotPower;
            BaseDamage = baseDamage;
            MinVerticalAngle = minVerticalAngle;
            MaxVerticalAngle = maxVerticalAngle;
            MaxHorizontalAngle = maxHorizontalAngle;
            TurnSpeed = turnSpeed;
            Variance = variance;
            Cost = cost;
        }
    }

}