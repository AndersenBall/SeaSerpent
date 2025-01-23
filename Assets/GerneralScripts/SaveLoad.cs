using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

public class SaveLoad : MonoBehaviour
{
    private static string currentFolder = "Default"; // Default save folder

    public static void SetSaveFolder(string folder)
    {
        currentFolder = folder;
        Debug.Log("Save folder set to: " + currentFolder);
    }

    public static string GetSaveDirectory()
    {
        return Application.persistentDataPath + $"/saves/{currentFolder}/";
    }

    public static void Save<T>(T objectToSave, string key)
    {
        string path = GetSaveDirectory();
        Directory.CreateDirectory(path);

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                DefaultMembersSearchFlags = System.Reflection.BindingFlags.NonPublic |
                                        System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.Instance
            }
        };

        string json = JsonConvert.SerializeObject(objectToSave, settings);

        File.WriteAllText(path + key + ".json", json);
        Debug.Log("Saved: " + key);
    }

    public static T Load<T>(string key)
    {
        string path = GetSaveDirectory() + key + ".json";

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found: {path}");
            return default;
        }

        string json = File.ReadAllText(path);

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        T returnValue = JsonConvert.DeserializeObject<T>(json, settings);

        Debug.Log("Loaded: " + key);
        return returnValue;
    }

    public static void DeleteSave(string key)
    {
        string path = GetSaveDirectory() + key + ".json";
        if (SaveExists(key))
        {
            File.Delete(path);
            Debug.Log($"Deleted save file: {path}");
        }
        else
        {
            Debug.LogWarning($"Save file not found for key: {key}");
        }
    }

    public static bool SaveExists(string key)
    {
        string path = GetSaveDirectory() + key + ".json";
        return File.Exists(path);
    }

    public static void SeriouslyDeleteAllSaveFiles()
    {
        string path = Application.persistentDataPath + "/saves/";
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
        Debug.Log("All save files deleted.");
    }
}
