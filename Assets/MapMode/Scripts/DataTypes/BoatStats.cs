[System.Serializable]
public class BoatStats
{
    public float speed;
    public float turnSpeed;
    public int health;
    public int cargoMax;
    public int maxSailorCount;

    public BoatStats(float speed, float turnSpeed, int health, int cargoMax, int maxSailorCount)
    {
        this.speed = speed;
        this.turnSpeed = turnSpeed;
        this.health = health;
        this.cargoMax = cargoMax;
        this.maxSailorCount = maxSailorCount;
    }
}
