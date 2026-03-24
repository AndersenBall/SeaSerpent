using System;
using System.Collections;
using System.Collections.Generic;
using MapMode.Scripts;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }
    public static event Action SaveInitiated;
    public static event Action LoadInitiated;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void SaveGame()
    {
        SaveInitiated?.Invoke();
        GameStateRepository.SaveCurrent();

        int i = 0;
        SaveLoad.Save(i, "GameMeta");
    }

    public static void LoadGame()
    {
        if (GameStateRepository.TryLoadCurrent())
        {
            LoadInitiated?.Invoke();
            return;
        }

        SaveGame();
        Debug.Log("No file to load");
    }

    [Obsolete("Runtime cleanup should happen via subscriber lifecycle (OnEnable/OnDisable or OnDestroy). Avoid clearing global event delegates.")]
    public static void ClearEvents() {
        SaveInitiated = null;
        LoadInitiated = null;
        
    }
}
