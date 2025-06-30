using UnityEngine;

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

[System.Serializable]
public class Cannon : IBoatComponent
{
    public string Name { get; private set; }
    public int Damage { get; private set; }

    public Cannon(string name, int damage)
    {
        Name = name;
        Damage = damage;
    }

    public void ApplyEffect(BoatStats stats)
    {
        //stats.firepower += Damage;
    }
}