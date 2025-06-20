using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSetCombat : MonoBehaviour
{
    // Start is called before the first frame update
    public int ShipNumber = 1;
    public int enemyShipNumber = 1;
    void Start()
    {

        Fleet playerFleet = new Fleet("england", "jalapeno");
        for (int i = 0; i < ShipNumber; i++){
            playerFleet.AddBoat(new Boat("p"+i, BoatType.Frigate));
        }
        PlayerGlobal.playerBoat = new Boat("Serpent", BoatType.Frigate);
        PlayerGlobal.playerBoat.SetBoatHealth(110);
        PlayerGlobal.playerBoat.AddSailor(new Sailor("tod",SailorType.Gunner));
        PlayerGlobal.playerBoat.AddSailor(new Sailor("tod",SailorType.Gunner));
        PlayerGlobal.playerBoat.AddSailor(new Sailor("tod",SailorType.Gunner));
        PlayerGlobal.playerBoat.AddSailor(new Sailor("tod",SailorType.Gunner));

        Fleet enemFleet = new Fleet("france", "loser");
        for (int i=0; i< enemyShipNumber; i++){
            enemFleet.AddBoat(new Boat("e"+i, BoatType.Frigate));
        }
        
        //enemFleet.AddBoat(new Boat("e3", "TradeShip"));

        SceneTransfer.playerFleet = playerFleet;
        SceneTransfer.enemyFleet = enemFleet;
        SceneManager.LoadScene("MainScene");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
