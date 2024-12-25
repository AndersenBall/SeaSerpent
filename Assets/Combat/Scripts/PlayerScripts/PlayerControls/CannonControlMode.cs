using UnityEngine;

public class CannonControlMode : ControlMode
{
    private HUDController hud;
    private PlayerTriggerController playerController;
    private CannonCameraControl cannonCameraControl;

    public CannonControlMode(HUDController hud, PlayerTriggerController playerController, CannonCameraControl cannonCameraControl)
    {
        this.hud = hud;
        this.playerController = playerController;
        this.cannonCameraControl = cannonCameraControl;

    }

    public override void EnterMode()
    {
        Debug.Log("Entered Cannon Control Mode");
        //hud.ShowCannonView(); // Show HUD for cannon controls
        playerController.activeCannon.SetLineActivity(true);
        playerController.activeCannon.isBeingWorkedOn = true;
        playerController.activeCannon.isManned = true;
        cannonCameraControl.EnterCannonMode(playerController.activeCannon.rotationPoint.transform);
        Cursor.visible = false; // Lock cursor for aiming
        Cursor.lockState = CursorLockMode.Locked;

    }

    public override void UpdateMode()
    {
        var activeCannon = playerController.activeCannon;
        
        // Use movement input axes for controlling cannon
        float horizontal = Input.GetAxis("Horizontal");
        int vertical = 0;
        if (Input.GetKey(KeyCode.UpArrow)) vertical = 1;
        if (Input.GetKey(KeyCode.DownArrow)) vertical = -1;

        if (horizontal != 0)
        {
            activeCannon.AdjustHorizontalAngle(horizontal); 
        }

        if (vertical != 0)
        {
            activeCannon.AdjustVerticalAngle(vertical); 
        }

        // Fire cannon with space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            activeCannon.Fire();
        }
    }

    public override void ExitMode()
    {
        Debug.Log("Exited Cannon Control Mode");
        //hud.HideCannonView(); 
        playerController.activeCannon.SetLineActivity(false);
        playerController.activeCannon.isBeingWorkedOn = false;
        cannonCameraControl.ExitCannonMode();
        playerController.activeCannon.StopWorkingOnCannon();
        Cursor.visible = true; // Unlock cursor
        Cursor.lockState = CursorLockMode.None;
    }
}

