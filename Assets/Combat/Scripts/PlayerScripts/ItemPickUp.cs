using System.Collections;
using System.Collections.Generic;
using Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame.Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame;
using UnityEngine;

//this script is used to cast a ray to  the ground and detect what object is hit. currently it is how the player picks up a cannon ball
public class ItemPickUp : MonoBehaviour
{
    public Camera fpsCam;
    public GameObject cannonBallPrefab;
    public Transform handTransform;
    public HUDController HUD;

    
    private GameObject cannonBall;
   

    public float range = 10f;
    private bool hasCannonBall=false;

    
    public bool GetCannonBallStatus() {
        return hasCannonBall;
    }

    public void RemoveCannonBall() {
        Destroy(cannonBall);
        HUD.cannonIconOff();
        hasCannonBall = false;
    }
    public void Detect()
    {
        RaycastHit hit;
        Debug.DrawRay(fpsCam.transform.position + fpsCam.transform.forward / 10, fpsCam.transform.forward, Color.green);
        if (Physics.Raycast(fpsCam.transform.position+fpsCam.transform.forward/10, fpsCam.transform.forward, out hit, range)) {
            Debug.Log("interacted with:" + hit.transform.tag);
            if (hit.transform.CompareTag("CannonBall")) {
                HandleCannonBallPickup();
            }
            if (hit.transform.CompareTag("RepairTask")) // Use a tag for objects that trigger the mini-game
            {
                OnRepairTaskRaycastHit(hit);
            }

           
        }
    }
 
    private void OnRepairTaskRaycastHit(RaycastHit hit)
    {
        Debug.Log("Starting mini-game");
        
        RepairTask repairTask = hit.transform.GetComponent<RepairTask>();
        if (repairTask != null){
            repairTask.startMiniGame();
        }
    }


    private void HandleCannonBallPickup()
    {
        if (!hasCannonBall)
        {
            HUD.cannonIconOn();
            Debug.Log("Grabbed cannonball");
            hasCannonBall = true;

            cannonBall = Instantiate(cannonBallPrefab, handTransform);
            cannonBall.transform.parent = handTransform;
        }
    }

}
