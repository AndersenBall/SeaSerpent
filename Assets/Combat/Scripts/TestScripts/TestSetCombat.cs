﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSetCombat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Fleet playerFleet = new Fleet("england", "jalapeno");
        for (int i = 0; i < 12; i++){
            playerFleet.AddBoat(new Boat("p"+i, "Frigate"));
        }
        PlayerGlobal.playerBoat = new Boat("Serpent", "Frigate");

        Fleet enemFleet = new Fleet("france", "loser");
        for (int i=0; i< 12; i++){
            enemFleet.AddBoat(new Boat("e"+i, "Frigate"));
        }
        
        //enemFleet.AddBoat(new Boat("e3", "TradeShip"));

        SceneTransfer.playerFleet = playerFleet;
        SceneTransfer.enemyFleet = enemFleet;
        SceneManager.LoadScene(1);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}