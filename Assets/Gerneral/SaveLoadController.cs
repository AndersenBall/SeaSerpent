using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadController : MonoBehaviour
{
    // Start is called before the first frame update
    
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
