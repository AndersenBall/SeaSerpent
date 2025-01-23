using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    
    private void Awake()
    {
        // Retrieve the current folder from SceneTransferMainMenu
        string folderFromTransfer = SceneTransferMainMenu.SaveFolder;

        // If no folder is set in SceneTransferMainMenu, fallback to default
        if (!string.IsNullOrEmpty(folderFromTransfer))
        {
            SaveLoad.SetSaveFolder(folderFromTransfer);
        }
        else
        {
            Debug.LogWarning("SceneTransferMainMenu.SaveFolder is empty. Using default folder.");
        }

        Debug.Log("SaveLoad: Current folder set to " + SaveLoad.GetSaveDirectory());
    }


    private void Start()
    {
        GameEvents.LoadGame();
        SceneTransfer.UpdateBoatsFromBattle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s")) {
            GameEvents.SaveGame();
        }
        if (Input.GetKeyDown("l")) {
            GameEvents.LoadGame();
        }
        if (Input.GetKeyDown("p")) {
            SaveLoad.SeriouslyDeleteAllSaveFiles();
        }
        
    }
}
