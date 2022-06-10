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

    public static void TransferToMap() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
        GameEvents.ClearActions();
        
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

        if (playerFleet != null) {
            GameObject player = GameObject.Find("PlayerBoat");
            player.GetComponent<PlayerFleetMapController>().SetFleet(playerFleet);
            Debug.Log(playerFleet.GetBoats()[0].GetBoatHealth()+ playerFleet.GetBoats()[0].boatName);
            playerFleet = null;
        }

        
    }
}
