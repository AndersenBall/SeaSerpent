using UnityEngine;

public class MapControlMode : ControlMode
{
    private HUDController hud;

    public MapControlMode(HUDController hud)
    {
        this.hud = hud;
    }

    public override void EnterMode()
    {
        Debug.Log("Entered Map Control Mode");
        hud.ShowMapView();
    }

    public override void UpdateMode()
    {
        
    }

    public override void ExitMode()
    {

    }
}

