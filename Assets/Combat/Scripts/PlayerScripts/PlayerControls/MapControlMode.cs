using UnityEngine;

public class MapControlMode : ControlMode
{
    private HUDController hud;
    private MapCamera cam;
    private RTSBoxSelection boxSelection;

    public MapControlMode(HUDController hud, MapCamera cam, RTSBoxSelection boxSelection)
    {
        this.hud = hud;
        this.cam = cam;
        this.boxSelection = boxSelection;
    }

    public override void EnterMode()
    {
        Debug.Log("Entered Map Control Mode");
        hud.ShowMapView();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cam.enabled = true;
        boxSelection.enabled = true;
    }

    public override void UpdateMode()
    {
        
    }

    public override void ExitMode()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam.enabled = false;
        boxSelection.enabled = false;
    }
}

