using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    // Start is called before the first frame update
    
    private void Start()
    {
        SceneTransfer.LoadGame();
        SceneTransfer.UpdateBoatsFromBattle();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s")) {
            SceneTransfer.SaveGame();
        }
        if (Input.GetKeyDown("l")) {
            SceneTransfer.LoadGame();
        }
        if (Input.GetKeyDown("p")) {
            SaveLoad.SeriouslyDeleteAllSaveFiles();
        }
        
    }
}
