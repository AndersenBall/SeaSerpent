using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MapMode.Scripts.DataTypes.boatComponents.Cannons;

public class TestSetCombat : MonoBehaviour
{
    // Start is called before the first frame update
    public int ShipNumber = 1;
    public int enemyShipNumber = 1;
    void Start()
    {

        Fleet playerFleet = new Fleet(Nation.Britain, "jalapeno");
        for (int i = 0; i < ShipNumber; i++){
            Boat newBoat = new Boat("p"+i, BoatType.Frigate);
            newBoat.SetCannon(new Cannon(CannonType.Carronade));
            newBoat.AddSailor(new Sailor("t",SailorType.Gunner));
            newBoat.AddSailor(new Sailor("t",SailorType.Gunner));
            playerFleet.AddBoat(newBoat);
        }
        

        Fleet enemFleet = new Fleet(Nation.Britain, "loser");
        for (int i=0; i< enemyShipNumber; i++){
            Boat newBoat = new Boat("e"+i, BoatType.Frigate);
            newBoat.AddSailor(new Sailor("t",SailorType.Gunner));
            newBoat.AddSailor(new Sailor("t",SailorType.Gunner));
            enemFleet.AddBoat(newBoat);
        }
        
        //enemFleet.AddBoat(new Boat("e3", "TradeShip"));

        SceneTransfer.playerFleet = playerFleet;
        SceneTransfer.enemyFleet = enemFleet;
        SceneManager.LoadScene("MainScene");

    }

   
}
