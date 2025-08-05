using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneSwap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("j")) {
            Fleet enemy = new Fleet(Nation.Spain,"The Strong");
            for (int i = 0; i < 10; i++) {
                enemy.AddBoat(new Boat("Gust"+i, BoatType.Frigate));
            }
            
            Fleet player = new Fleet(Nation.PlayerNation, "Ball");
            for (int i = 0; i < 10; i++) {
                player.AddBoat(new Boat("Gust" + i, BoatType.Frigate));
            }
            SceneTransfer.enemyFleet = enemy;
            SceneTransfer.playerFleet = player;
            SceneManager.LoadScene("MainScene");
        }
    }
}
