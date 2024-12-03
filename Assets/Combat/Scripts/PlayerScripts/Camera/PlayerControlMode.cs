using ECM.Components;
using UnityEngine;

public class PlayerControlMode : ControlMode
{
    private CharacterPositionInterface characterPositionInterface;
    private HUDController hud;
    private MouseLook mouseLook;
    private PlayerTriggerController playerTriggerController;
    public PlayerControlMode(PlayerTriggerController playerTriggerController, CharacterPositionInterface characterPositionInterface, HUDController hud,MouseLook mouseLook)
    {
        this.playerTriggerController = playerTriggerController;
        this.characterPositionInterface = characterPositionInterface;
        this.hud = hud;
        this.mouseLook = mouseLook;
    }

    public override void EnterMode()
    {
        hud.ShowFirstPersonView();
        characterPositionInterface.SetFirstPerControllerActive(true);
        mouseLook.enabled = true;
        playerTriggerController.enabled = true;
        playerTriggerController.SetHandGun(true);

        Debug.Log("entered player control mode");
    }

    public override void UpdateMode()
    {
        
    }

    public override void ExitMode()
    {
        characterPositionInterface.SetFirstPerControllerActive(false);
        mouseLook.enabled = false;
        playerTriggerController.SetHandGun(false);
        playerTriggerController.enabled = false;
    }
}


