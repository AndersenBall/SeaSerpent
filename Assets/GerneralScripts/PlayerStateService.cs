using UnityEngine;

public static class PlayerStateService
{
    public static float Money
    {
        get => GameStateRepository.Current.Player.Money;
        set => GameStateRepository.Current.Player.Money = value;
    }

    public static Boat ActiveBoat
    {
        get => GameStateRepository.Current.Player.ActiveBoat;
        set => GameStateRepository.Current.Player.ActiveBoat = value;
    }

    public static Fleet PlayerFleet
    {
        get => GameStateRepository.Current.Player.Fleet;
        set => GameStateRepository.Current.Player.Fleet = value;
    }

    public static float[] MapPosition
    {
        get => GameStateRepository.Current.Player.MapPosition;
        set => GameStateRepository.Current.Player.MapPosition = value;
    }

    public static bool TrySpendMoney(float amount)
    {
        if (amount < 0f)
        {
            Debug.LogWarning($"TrySpendMoney called with negative amount: {amount}");
            return false;
        }

        if (Money < amount)
        {
            return false;
        }

        Money -= amount;
        return true;
    }

    public static void AddMoney(float amount)
    {
        Money += amount;
    }
}
