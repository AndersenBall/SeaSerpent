using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MeetShipUI : MonoBehaviour
{
    public GameObject panel;
    public PlayerFleetMapController playerFleetMap;
    private Fleet oppositeFleet;
    private Fleet playerFleet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ContactShip(Fleet f) {
        panel.SetActive(true);
        Transform enemyFleetInfo = transform.Find("Panel/Image/EnemyFleetInfo");
        enemyFleetInfo.GetComponent<UnityEngine.UI.Text>().text = f.commander + " Size of Fleet: " + f.getNumberBoats() + " Nationality: " + f.Nationality;
        Time.timeScale = 0f;
        oppositeFleet = f;
    }

    public void CloseContact() {
        panel.SetActive(false);
        Time.timeScale = 1f;
        
        
        oppositeFleet = null;

    }

    public void FightFleet() {
        panel.SetActive(false);
        Time.timeScale = 1f;
        SceneTransfer.playerFleet = playerFleetMap.GetFleet();
        SceneTransfer.enemyFleet = oppositeFleet;
        GameEvents.SaveInitiated();
        SceneManager.LoadScene(1);
    }
}
