using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

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
        string path = Application.persistentDataPath + $"/saves/{currentFolder}/";
        Directory.CreateDirectory(path);
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream fileStream = new FileStream(path + key + ".txt", FileMode.Create))
        {
            formatter.Serialize(fileStream, objectToSave);
        }
        Debug.Log("saved!: " + key);
    }

    public static T Load<T>(string key)
    {
        string path = Application.persistentDataPath + $"/saves/{currentFolder}/";
        BinaryFormatter formatter = new BinaryFormatter();
        T returnValue = default(T);
        using (FileStream fileStream = new FileStream(path + key + ".txt", FileMode.Open))
        {
            returnValue = (T)formatter.Deserialize(fileStream);
        }
        Debug.Log("loaded: " + key);
        return returnValue;
    }

    public static void DeleteSave(string key)
    {
        string path = Application.persistentDataPath + $"/saves/{currentFolder}/{key}.txt";
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
        string path = Application.persistentDataPath + $"/saves/{currentFolder}/{key}.txt";
        return File.Exists(path);
    }


    public static void SeriouslyDeleteAllSaveFiles()
    {
        string path = Application.persistentDataPath + "/saves/";
        DirectoryInfo directory = new DirectoryInfo(path);
        directory.Delete(true);
        Directory.CreateDirectory(path);
    }
}
