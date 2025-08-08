using UnityEngine.Serialization;

[System.Serializable]
public class BoatStats
{
    public float speed;
    public float turnSpeed;
    [FormerlySerializedAs("health")] public int maxHealth;
    public int maxSailHealth;
    public int cargoMax;
    public int maxSailorCount;
    public int boatCost;

    public BoatStats(float speed, float turnSpeed, int maxHealth, int cargoMax, int maxSailorCount, int boatCost)
    {
        this.speed = speed;
        this.turnSpeed = turnSpeed;
        this.maxHealth = maxHealth;
        this.maxSailHealth = 100;
        this.cargoMax = cargoMax;
        this.maxSailorCount = maxSailorCount;
        this.boatCost = boatCost;

    }

    public override string ToString()
    {
        return $"Speed: {speed}, Turn Speed: {turnSpeed}, Health: {maxHealth}, " +
               $"Cargo Max: {cargoMax}, Max Sailor Count: {maxSailorCount}, Boat Cost: {boatCost}";
    }
}
