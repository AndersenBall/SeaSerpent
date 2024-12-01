//public class playercontrolmode : controlmode
//{
//    private playertriggercontroller player;

//    public playercontrolmode(playertriggercontroller player)
//    {
//        this.player = player;
//    }

//    public override void entermode()
//    {
//        player.enableplayercontrols();
//        debug.log("entered player control mode");
//    }

//    public override void updatemode()
//    {
//        // handle player input
//        if (player.guncontrol != null)
//        {
//            if (input.getkeydown("q") && player.guncontrol.getloadstatus())
//            {
//                player.guncontrol.fire();
//            }
//            if (input.getkeydown("e") && !player.guncontrol.getloadstatus() && player.itempickup.getcannonballstatus())
//            {
//                player.guncontrol.loadgun();
//                player.itempickup.removecannonball();
//            }
//            if (input.getkeydown("u"))
//            {
//                player.guncontrol.rotatebarrel();
//            }
//        }
//    }

//    public override void exitmode()
//    {
//        player.disableplayercontrols();
//    }
//}


