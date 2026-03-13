using UnityEngine;

public static class GameStateRepository
{
    private const string SaveKey = "GameState";
    private static GameState _current;

    public static GameState Current
    {
        get
        {
            if (_current == null)
            {
                _current = GameState.CreateDefault();
            }

            EnsureValidCurrent();
            return _current;
        }
    }

    public static void SetCurrent(GameState state)
    {
        _current = state ?? GameState.CreateDefault();
        EnsureValidCurrent();
    }

    public static void ResetToDefault()
    {
        _current = GameState.CreateDefault();
    }

    public static void SaveCurrent()
    {
        SaveLoad.Save(Current, SaveKey);
        Debug.Log("GameState saved successfully.");
    }

    public static bool TryLoadCurrent()
    {
        if (!SaveLoad.SaveExists(SaveKey))
        {
            return false;
        }

        var loaded = SaveLoad.Load<GameState>(SaveKey);
        SetCurrent(loaded);
        Debug.Log("GameState loaded successfully.");
        return true;
    }

    private static void EnsureValidCurrent()
    {
        if (_current.Player == null)
        {
            _current.Player = new PlayerState();
        }

        if (_current.Player.MapPosition == null || _current.Player.MapPosition.Length < 3)
        {
            _current.Player.MapPosition = new float[] { 0f, 0f, 0f };
        }
    }
}
