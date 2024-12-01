//public class BoatControlMode : ControlMode
//{
//    private PlayerTriggerController player;

//    public BoatControlMode(PlayerTriggerController player)
//    {
//        this.player = player;
//    }

//    public override void EnterMode()
//    {
//        player.EnterBoatControls();
//        Debug.Log("Entered Boat Control Mode");
//    }

//    public override void UpdateMode()
//    {
//        // Exit boat control
//        if (Input.GetKeyDown("e"))
//        {
//            player.GameManager.SetMode(new PlayerControlMode(player));
//        }
//    }

//    public override void ExitMode()
//    {
//        player.ExitBoatControls();
//    }
//}

