using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
  
    public static System.Action SaveInitiated;
    public static System.Action LoadInitiated;

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
        else { Debug.Log("No file to load"); }
    }

    public static void ClearActions() {
        SaveInitiated = null;
        LoadInitiated = null;
    }
}
