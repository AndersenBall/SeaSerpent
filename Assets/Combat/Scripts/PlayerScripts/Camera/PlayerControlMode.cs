using UnityEngine;

public class PlayerControlMode : ControlMode
{
    private CharacterPositionInterface characterPositionInterface;
    private HUDController hud;

    public PlayerControlMode(CharacterPositionInterface characterPositionInterface, HUDController hud)
    {
        this.characterPositionInterface = characterPositionInterface;
        this.hud = hud;
    }

    public override void EnterMode()
    {
        hud.ShowFirstPersonView();
        characterPositionInterface.SetFirstPerControllerActive(true);
        Debug.Log("entered player control mode");
    }

    public override void UpdateMode()
    {
        
    }

    public override void ExitMode()
    {
        characterPositionInterface.SetFirstPerControllerActive(false);

    }
}


