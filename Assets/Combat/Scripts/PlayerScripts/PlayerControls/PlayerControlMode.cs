using ECM.Components;
using UnityEngine;

public class PlayerControlMode : ControlMode
{
    private CharacterPositionInterface characterPositionInterface;
    private HUDController hud;
    private MouseLook mouseLook;
    private MouseLookPlayer mouseLookPlayer;
    private PlayerTriggerController playerTriggerController;
    public PlayerControlMode(PlayerTriggerController playerTriggerController, CharacterPositionInterface characterPositionInterface, HUDController hud,MouseLook mouseLook, MouseLookPlayer mouseLookPlayer)
    {
        this.playerTriggerController = playerTriggerController;
        this.characterPositionInterface = characterPositionInterface;
        this.hud = hud;
        this.mouseLook = mouseLook;
        this.mouseLookPlayer = mouseLookPlayer;
    }

    public override void EnterMode()
    {
        hud.ShowFirstPersonView();
        characterPositionInterface.SetFirstPerControllerActive(true);
        mouseLook.enabled = true;
        mouseLookPlayer.gameObject.SetActive(true);
        //playerTriggerController.enabled = true;
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
        mouseLookPlayer.gameObject.SetActive(false);
        playerTriggerController.SetHandGun(false);
        //playerTriggerController.enabled = false;
    }
}


