using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

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
        Transform fleetCommander = transform.Find("Panel/ParchmentPanel/Image/FleetCommander");
        Transform fleetNum = transform.Find("Panel/ParchmentPanel/Image/FleetNum");
        Transform fleetNationality = transform.Find("Panel/ParchmentPanel/Image/FleetNationality");
        fleetCommander.GetComponent<TMP_Text>().text = " Commander: " + f.commander;
        fleetNum.GetComponent<TMP_Text>().text = " Number of Ships: " + f.getNumberBoats();
        fleetNationality.GetComponent<TMP_Text>().text = " Nationality: " + f.Nationality;
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
        SceneManager.LoadScene(2);
    }
}
