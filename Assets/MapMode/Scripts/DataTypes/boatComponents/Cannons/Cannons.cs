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
        
        public CannonType CannonType { get; private set; }

        public Cannon(CannonType type)
        {
            CannonType = type;
            UpgradeLevel = 0;
            
            switch(type)
            {
                case CannonType.LongGun:
                    ShotWeight = 24;
                    ShotPower = 200f;
                    BaseDamage = 0;
                    Cost = 1000;
                    MinVerticalAngle = -20f;
                    MaxVerticalAngle = 10f;
                    MaxHorizontalAngle = 25f;
                    TurnSpeed = 5f;
                    break;
                case CannonType.Carronade:
                    ShotWeight = 32;
                    ShotPower = 100f;
                    BaseDamage = 20;
                    Cost = 1200;
                    MinVerticalAngle = -15f;
                    MaxVerticalAngle = 10f;
                    MaxHorizontalAngle = 0f;
                    TurnSpeed = 3f;
                    break;
                case CannonType.Mortar:
                    ShotWeight = 64;
                    ShotPower = 50f;
                    BaseDamage = 30;
                    Cost = 2000;
                    MinVerticalAngle = -80f;
                    MaxVerticalAngle = -25f;
                    MaxHorizontalAngle = 360f;
                    TurnSpeed = 15f;
                    break;
                case CannonType.FlameThrower:
                    ShotWeight = 0;
                    ShotPower = 40f;
                    BaseDamage = 50;
                    Cost = 1500;
                    MinVerticalAngle = -15f;
                    MaxVerticalAngle = 15f;
                    MaxHorizontalAngle = 30f;
                    TurnSpeed = 15f;
                    break;
                case CannonType.GrappleHook:
                    ShotWeight = 0;
                    ShotPower = 100f;
                    BaseDamage = 10;
                    Cost = 800;
                    MinVerticalAngle = -30f;
                    MaxVerticalAngle = 60f;
                    MaxHorizontalAngle = 90f;
                    TurnSpeed = 20f;
                    break;
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