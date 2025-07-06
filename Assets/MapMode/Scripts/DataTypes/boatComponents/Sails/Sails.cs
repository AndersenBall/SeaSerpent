namespace MapMode.Scripts.DataTypes.boatComponents.Sails
{
    [System.Serializable]
    public class Sails : IBoatComponent
    {
        public string Name { get; private set; }
        public float SpeedBoost { get; private set; }

        public Sails(string name, float speedBoost)
        {
            Name = name;
            SpeedBoost = speedBoost;
        }

        public void ApplyEffect(BoatStats stats)
        {
            stats.speed += SpeedBoost;
        }
    }
}