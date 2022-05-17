using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
  
    public static System.Action SaveInitiated;
    public static System.Action LoadInitiated;


    public static void OnSaveInitiated()
    {
        SaveInitiated?.Invoke();
    }
    public static void OnLoadInitiated()
    {
        Time.timeScale = 0;
        LoadInitiated?.Invoke();
        Time.timeScale = 1;
    }
    public static void ClearActions() {
        SaveInitiated = null;
        LoadInitiated = null;
    }
}
