using System.Collections;
using System.Collections.Generic;
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

 
    void Update()
    {
        /*
        if (Input.GetKeyDown("e")) {
            Detect();
        }
        if (Input.GetKeyDown("") && cannonBall != null) {
            RemoveCannonBall();
            HUD.cannonIconOff();
        }
        */
    }
    public bool GetCannonBallStatus() {
        return hasCannonBall;
    }

    public void RemoveCannonBall() {
        Destroy(cannonBall);
        HUD.cannonIconOff();
        hasCannonBall = false;
    }
    public void PickUpCannonBall()
    {
        RaycastHit hit;
        Debug.DrawRay(fpsCam.transform.position + fpsCam.transform.forward / 10, fpsCam.transform.forward, Color.green);
        if (Physics.Raycast(fpsCam.transform.position+fpsCam.transform.forward/10, fpsCam.transform.forward, out hit, range)) {
            //Debug.Log(hit.transform.tag);
            if (hit.transform.CompareTag("CannonBall") && !hasCannonBall) {

                HUD.cannonIconOn();
                Debug.Log("grabbed cannon Ball");
                hasCannonBall = true;

                cannonBall = Instantiate(cannonBallPrefab, handTransform);
                cannonBall.transform.parent= handTransform;
            }
           
        }
    }
}
