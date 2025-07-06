namespace MapMode.Scripts.DataTypes.boatComponents
{
    [System.Serializable]
    public class Cannon : IBoatComponent
    {
        public int ShotWeight { get; private set; }
        public float Range { get; private set; }
        public int BaseDamage { get; private set; }
        public int UpgradeLevel { get; private set; }
        public int Damage { get; private set; }
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
                    Range = 600f;
                    BaseDamage = 80;
                    Cost = 1000;
                    break;
                case CannonType.Carronade:
                    ShotWeight = 32;
                    Range = 300f;
                    BaseDamage = 120;
                    Cost = 1200;
                    break;
                case CannonType.Mortar:
                    ShotWeight = 64;
                    Range = 800f;
                    BaseDamage = 150;
                    Cost = 2000;
                    break;
                case CannonType.FlameThrower:
                    ShotWeight = 0;
                    Range = 50f;
                    BaseDamage = 50;
                    Cost = 1500;
                    break;
                case CannonType.GrappleHook:
                    ShotWeight = 0;
                    Range = 100f;
                    BaseDamage = 10;
                    Cost = 800;
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
            return $"Cannon Type: {CannonType}, Shot Weight: {ShotWeight} lb, Range: {Range} m, Base Damage: {BaseDamage}, Upgrade Level: {UpgradeLevel}, Final Damage: {Damage}, Cost: {Cost}";
        }

        public void ApplyEffect(BoatStats stats)
        {
            return;
        }
    }
}