using UnityEngine;

public class BoatControlMode : ControlMode
{
    public CharacterPositionInterface characterPositionInterface;
    private BoatControls boatControls;
    private HUDController hud;

    private float sideways = 0;
    private float throttle = 0;

    public BoatControlMode(CharacterPositionInterface characterPositionInterface, BoatControls boatControls, HUDController hud)
    {
        this.characterPositionInterface = characterPositionInterface;
        this.boatControls = boatControls;
        this.hud = hud;
    }

    public override void EnterMode()
    {
        Debug.Log("Entered Boat Control Mode");
        boatControls.SetIsPlayerDriving(true);
        characterPositionInterface.SetFirstPerControllerActive(false);
        hud.ShowOverheadView();
    }

    public override void UpdateMode()
    {
        // Handle boat turning
        sideways = (Input.GetKey(KeyCode.A) ? -1f : 0f) +
                   (Input.GetKey(KeyCode.D) ? 1f : 0f);
        boatControls.SetTurn(sideways);

        // Handle throttle adjustments
        if (Input.GetKeyDown(KeyCode.W))
        {
            throttle += 0.1f;
            throttle = Mathf.Clamp(throttle, -1f, 1f);
            boatControls.SetForward(throttle);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            throttle -= 0.1f;
            throttle = Mathf.Clamp(throttle, -1f, 1f);
            boatControls.SetForward(throttle);
        }


    }

    public override void ExitMode()
    {
        Debug.Log("Exited Boat Control Mode");
        boatControls.SetIsPlayerDriving(false);

    }
}
