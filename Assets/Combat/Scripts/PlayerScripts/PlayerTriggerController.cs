using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//this script is in charge of all triggers the player comes in contact with
public class PlayerTriggerController : MonoBehaviour
{
    //player controls
    public HUDController hud;
    public CannonInterface activeCannon;
    private ItemPickUp itemPickUp;
    private FireGun handGunControl;
    


    private Collider triggerInsideOf = null;


    private BoatControls boatControls;
    public ShipCrewCommand shipCrewCommand;
    public ShipAmunitionInterface shipAmunitionInterface;

    private string currentCommand = "nothing";


    private void Start()
    {
        itemPickUp = gameObject.GetComponent<ItemPickUp>();
  
        handGunControl = gameObject.GetComponentInChildren<FireGun>();

    }

    private void Update()
    {

        //(float, float) values = boatControls.GetBoatSpeed();
        //hud.UpdateSailStength((Mathf.Round(values.Item1 * 10) / 10f, Mathf.Round(values.Item2 * 10) / 10f));
        //if inside of a cannons trigger
        if (activeCannon != null) {
        
            if (Input.GetKeyDown("e") && !activeCannon.GetLoadStatus() && itemPickUp.GetCannonBallStatus()) {
                activeCannon.LoadGun();
                itemPickUp.RemoveCannonBall();
            }
            

        }


        //control boat
        if (Input.GetKeyDown("e")) {
            itemPickUp.PickUpCannonBall();
            
            
            
        }

        if (shipCrewCommand != null) {
            shipCrewCommand.SetCannonSets();

            if (currentCommand == "fireCommand") { 
                //shipCrewCommand.SetCannonSets();

                float multiplier = Input.GetKeyDown(KeyCode.LeftShift) ? .5f : 1;
                if (Input.GetKeyDown("v")){
                    Debug.Log("Log:PlayerTrigger:+ pressed");
                    shipCrewCommand.AdjustCannonAnglePredictions(.5f*multiplier);
                }
                if (Input.GetKeyDown("c")){
                    Debug.Log("Log:PlayerTrigger: - pressed");
                    shipCrewCommand.AdjustCannonAnglePredictions(-.5f*multiplier);
                }
            }
            

            if (Input.GetKeyDown("f"))
            {
                if (currentCommand == "fireCommand")
                {
                    shipCrewCommand.FireCannons();
                    NewCommand("nothing");
                }
                else
                {
                    shipCrewCommand.AdjustCannonAnglesPlayer();
                    NewCommand("fireCommand");
                    hud.CannonGroupHelpOn();

                }
            }
            if (Input.GetKeyDown("g"))
            {
                
                Debug.Log("Fire at will enabled");
                shipCrewCommand.FireAtWill();


            }

            if (Input.GetKeyDown("r"))
            {
                shipCrewCommand.ReloadCannons();
                NewCommand("Reload");
                shipCrewCommand.DeactivatePredictionLines();
            }

            

        }

    }
    public void SetUpBoat(BoatControls bcontrols){
        boatControls = bcontrols;
        shipCrewCommand = boatControls.GetComponent<ShipCrewCommand>();
        shipAmunitionInterface = boatControls.GetComponent<ShipAmunitionInterface>();
        boatControls.GetComponent<BoatAI>().playerOnBoard = true;
    }

    private void NewCommand(string newCommand) {
        shipCrewCommand.ClearCannons();
        //shipCrewCommand.DeactivatePredictionLines();
        hud.CannonGroupHelpOff();
        currentCommand = newCommand;

    }   
    
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Entered Trigger of " + other.transform.name);
        if (other.CompareTag("Cannon")) {
            hud.cannonKeyOn();
            activeCannon = other.gameObject.GetComponent<CannonInterface>();
        }
        if (other.CompareTag("steeringWheel")) {
            triggerInsideOf = other;
            Debug.Log("inside of steering Wheel");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cannon")) {
            //Debug.Log("Exited Trigger of " + other.transform.name);
            hud.cannonKeyOff();
            activeCannon = null;
        }
        if (other.CompareTag("steeringWheel")) {
            triggerInsideOf = null;
           // Debug.Log("Exit steering wheel");
        }
    }



    public void SetHandGun(bool value)
    {
        handGunControl.enabled = value;
        Debug.Log("disable hand gun:" + value);

    }

    

}
