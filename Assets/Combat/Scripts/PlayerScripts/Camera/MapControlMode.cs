using UnityEngine;

public class MapControlMode : ControlMode
{
    private HUDController hud;
    private MapCamera cam;

    public MapControlMode(HUDController hud, MapCamera cam)
    {
        this.hud = hud;
        this.cam = cam;
    }

    public override void EnterMode()
    {
        Debug.Log("Entered Map Control Mode");
        hud.ShowMapView();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cam.enabled = true;
    }

    public override void UpdateMode()
    {
        
    }

    public override void ExitMode()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam.enabled = false;
    }
}

