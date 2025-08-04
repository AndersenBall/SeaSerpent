using System;
using System.Collections;
using System.Collections.Generic;
using MapMode.Scripts;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance { get; private set; }
    public static System.Action SaveInitiated;
    public static System.Action LoadInitiated;
    
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
        int i = 0;
        SaveLoad.Save(i, "GameMeta");
        GameEvents.SaveInitiated.Invoke();
    }
    public static void LoadGame()
    {
        if (SaveLoad.SaveExists("GameMeta"))
        {
            GameEvents.LoadInitiated.Invoke();
        }
        else { 
            SaveGame(); 
            Debug.Log("No file to load"); 
        }
    }

    public static void ClearEvents() {
        SaveInitiated = null;
        LoadInitiated = null;
        
    }
}
