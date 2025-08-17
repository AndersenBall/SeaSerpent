using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransfer : MonoBehaviour
{

    public static Fleet playerFleet { get; set; }
    public static Fleet enemyFleet { get; set; }
    

    public static void Clear() {
        playerFleet = null;
        enemyFleet = null;
    }
    public static void TransferToTownUI() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("TownMenu");
        GameEvents.ClearEvents();
    }

    public static void TransferToMap() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("IslandView");
        GameEvents.ClearEvents();
        
    }

    public static void UpdateBoatsFromBattle() {//calleed after the world map is loaded back
        
        if (enemyFleet != null) {
            GameObject boats = GameObject.Find("Boats");
            FleetMapController[] fleetList = boats.GetComponentsInChildren<FleetMapController>();
            foreach (FleetMapController fleet in fleetList) {
                if (fleet.GetFleet().FleetID == enemyFleet.FleetID) {
                    fleet.SetFleet(enemyFleet);
                    Debug.Log("Fleet updated from battle" + enemyFleet.FleetID + enemyFleet.commander);
                    if (enemyFleet.getNumberBoats() == 0) {
                        Destroy(fleet.gameObject);
                    }
                    enemyFleet = null;
                    break;
                }
            }
        }

        
    }
}
