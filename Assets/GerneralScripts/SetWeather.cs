using System.Collections;
using System.Collections.Generic;
using Enviro;
using UnityEngine;

public class SetWeather : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Enviro.EnviroManager.instance == null)
        {
            Debug.LogError("EnviroManager.instance is null! Please ensure that Enviro Manager is properly set up in the scene.");
            return;
        }

        var fogSettings = Enviro.EnviroManager.instance.Fog.Settings;
        fogSettings.fog = true;
        fogSettings.volumetrics = true;
        fogSettings.quality = Enviro.EnviroFogSettings.Quality.Medium;
    
        Debug.Log($"Fog Enabled: {fogSettings.fog}, Volumetrics: {fogSettings.volumetrics}, Quality: {fogSettings.quality}");

    }

}
