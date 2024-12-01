//using UnityEngine;

//public class MapControlMode : ControlMode
//{
//    private PlayerTriggerController player;

//    public MapControlMode(PlayerTriggerController player)
//    {
//        this.player = player;
//    }

//    public override void EnterMode()
//    {
//        Debug.Log("Entered Map Control Mode");
//        player.HUD.ShowMapView();
//    }

//    public override void UpdateMode()
//    {
//        if (Input.GetKeyDown(KeyCode.M))
//        {
//            player.GameManager.ToggleMapMode(new MapControlMode(player), new PlayerControlMode(player));
//        }
//    }

//    public override void ExitMode()
//    {
//        player.HUD.HideMapView();
//    }
//}

