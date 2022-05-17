using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenPlayerMenu : MonoBehaviour
{
    // Start is called before the first frame update
    InventoryUI inventory;
    
    void Start()
    {
        inventory = GameObject.Find("Canvas/PlayerUI").GetComponent<InventoryUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("i")) {
            if (inventory.GetState() != "Inventory") {
                inventory.ChangeState("Inventory");
            }
            else {
                inventory.ChangeState("None");
            }
        }
    }
}
