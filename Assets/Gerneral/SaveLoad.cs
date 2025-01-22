using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Binder = new PolymorphicBinder(); // Add support for polymorphic types

        using (FileStream fileStream = new FileStream(path + key + ".txt", FileMode.Create))
        {
            formatter.Serialize(fileStream, objectToSave);
        }
        Debug.Log("Saved: " + key);
    }

    public static T Load<T>(string key)
    {
        string path = GetSaveDirectory() + key + ".txt";

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file not found: {path}");
            return default;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Binder = new PolymorphicBinder(); // Add support for polymorphic types

        T returnValue;

        using (FileStream fileStream = new FileStream(path, FileMode.Open))
        {
            returnValue = (T)formatter.Deserialize(fileStream);
        }

        Debug.Log("Loaded: " + key);
        return returnValue;
    }

    public static void DeleteSave(string key)
    {
        string path = GetSaveDirectory() + key + ".txt";
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
        string path = GetSaveDirectory() + key + ".txt";
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

    // Custom binder to support polymorphic serialization
    private sealed class PolymorphicBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            var type = Type.GetType($"{typeName}, {assemblyName}");
            if (type == null)
            {
                Debug.LogError($"Failed to bind type: {typeName}, {assemblyName}");
            }
            return type;
        }
    }
}
