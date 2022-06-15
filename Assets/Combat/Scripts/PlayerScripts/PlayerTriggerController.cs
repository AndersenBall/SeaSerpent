using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//this script is in charge of all triggers the player comes in contact with
public class PlayerTriggerController : MonoBehaviour
{
    //player controls
    public HUDController hud;
    private CannonInterface gunControl;
    private ItemPickUp itemPickUp;
    private FireGun handGunControl;
    
    private PlayerMovementOnBoat playerControls;
    // public MouseLook mouseLook;

    private bool drivingBoat = false;
    private Collider triggerInsideOf = null;
    private Collider steeringWheelTrigger = null;

    private BoatControls boatControls;
    public ShipCrewCommand shipCrewCommand;
    public ShipAmunitionInterface shipAmunitionInterface;
    private bool settingShipCannonGroups = false;
    private bool settingShipCannonAngle = false;
    private string currentCommand = "nothing";


    private void Start()
    {
        itemPickUp = gameObject.GetComponent<ItemPickUp>();
        playerControls = gameObject.GetComponent<PlayerMovementOnBoat>();
        handGunControl = gameObject.GetComponentInChildren<FireGun>();
        //boatControls = gameObject.GetComponentInParent<BoatControls>();
    }

    private void Update()
    {
        (float, float) values = boatControls.GetBoatSpeed();
        hud.UpdateSailStength((Mathf.Round(values.Item1 * 10) / 10f, Mathf.Round(values.Item2 * 10) / 10f));
        //if inside of a cannons trigger
        if (gunControl != null) {
            if (Input.GetKeyDown("q") && gunControl.GetLoadStatus()) {
                gunControl.Fire();
            }
            if (Input.GetKeyDown("e") && !gunControl.GetLoadStatus() && itemPickUp.GetCannonBallStatus()) {
                gunControl.LoadGun();
                itemPickUp.RemoveCannonBall();
            }
            if (Input.GetKeyDown("u")) {
                gunControl.RotateBarrel();
            }
        }


        //control boat
        if (Input.GetKeyDown("e")) {
            itemPickUp.PickUpCannonBall();
            if (drivingBoat) {
                EnterPlayerControls(steeringWheelTrigger);
                drivingBoat = false;
                steeringWheelTrigger = null;
            }
            
            else if (triggerInsideOf != null) {
                if (triggerInsideOf.CompareTag("steeringWheel")) {
                    EnterBoatControls(triggerInsideOf);
                    drivingBoat = true;
                    steeringWheelTrigger = triggerInsideOf;
                }
            }
        }

        if (shipCrewCommand != null) {
            if (currentCommand == "fireCommand") { 
                shipCrewCommand.SetCannonSets();

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
            gunControl = other.gameObject.GetComponent<CannonInterface>();
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
            gunControl = null;
        }
        if (other.CompareTag("steeringWheel")) {
            triggerInsideOf = null;
           // Debug.Log("Exit steering wheel");
        }
    }

    private void EnterBoatControls(Collider other) {
        boatControls = other.transform.parent.parent.parent.GetComponent<BoatControls>();
        boatControls.SetIsPlayerDriving(true);
        handGunControl.enabled = false;
        playerControls.ActiveBoatControls(boatControls);
        Debug.Log("we have entered boat controls");

        hud.ShowOverheadView();
    }

    private void EnterPlayerControls(Collider other)
    {
        boatControls = other.transform.parent.parent.parent.GetComponent<BoatControls>();
        boatControls.SetIsPlayerDriving(false);
        handGunControl.enabled = true;
        playerControls.SetIsActive(true);

        hud.ShowFirstPersonView();
    }

}
