using System;
using UnityEngine;

[Serializable]
public class GameState
{
    public PlayerState Player = new PlayerState();

    public static GameState CreateDefault()
    {
        return new GameState
        {
            Player = new PlayerState
            {
                Money = 0f,
                Fleet = null,
                ActiveBoat = null,
                MapPosition = new float[] { 0f, 0f, 0f }
            }
        };
    }
}

[Serializable]
public class PlayerState
{
    public float Money;
    public Fleet Fleet;
    public Boat ActiveBoat;
    public float[] MapPosition = new float[] { 0f, 0f, 0f };
}
