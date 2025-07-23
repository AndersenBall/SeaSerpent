namespace MapMode.Scripts.DataTypes.boatComponents.Cannons
{
    [System.Serializable]
    public class Cannon : IBoatComponent
    {
        public int ShotWeight { get; private set; }
        public float ShotPower { get; private set; }
        public int BaseDamage { get; private set; }
        public int UpgradeLevel { get; private set; }
        public int Damage { get; private set; }
        public float MinVerticalAngle { get; private set; }
        public float MaxVerticalAngle { get; private set; }
        public float TurnSpeed { get; private set; }
        public float MaxHorizontalAngle { get; private set; }
        public int Cost { get; private set; }
        
        public float Variance { get; private set; }
        
        public CannonType CannonType { get; private set; }

        public Cannon(CannonType type)
        {
            CannonType = type;
            UpgradeLevel = 0;
            
            if (CannonStatsDatabase.BaseStats.TryGetValue(type, out CannonStats stats))
            {
                ShotWeight = stats.ShotWeight;
                ShotPower = stats.ShotPower;
                BaseDamage = stats.BaseDamage;
                Cost = stats.Cost;
                MinVerticalAngle = stats.MinVerticalAngle;
                MaxVerticalAngle = stats.MaxVerticalAngle;
                MaxHorizontalAngle = stats.MaxHorizontalAngle;
                TurnSpeed = stats.TurnSpeed;
                Variance = stats.Variance;
            
            }
            
            CalculateDamage();
        }

        public void Upgrade()
        {
            UpgradeLevel++;
            CalculateDamage();
        }

        public void CalculateDamage()
        {
            Damage = BaseDamage + (int)(BaseDamage * 0.1f * UpgradeLevel);
        }

        public override string ToString()
        {
            return $"Cannon Type: {CannonType}, Shot Weight: {ShotWeight} lb, Range: {ShotPower} m, Base Damage: {BaseDamage}, Upgrade Level: {UpgradeLevel}, Final Damage: {Damage}, Cost: {Cost}";
        }

        public void ApplyEffect(BoatStats stats)
        {
            return;
        }
    }
}