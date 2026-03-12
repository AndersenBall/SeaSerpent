using System.Collections;
using System.Collections.Generic;
using GerneralScripts.BattleManagement;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeetShipUI : MonoBehaviour
{
    public GameObject panel;
    public PlayerFleetMapController playerFleetMap;
    private Fleet oppositeFleet;

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

        var playerFleet = playerFleetMap.GetFleet();
        if (playerFleet == null || oppositeFleet == null)
        {
            Debug.LogWarning("Cannot start battle: missing player or enemy fleet reference.");
            return;
        }

        if (BattleManager.Instance == null)
        {
            new GameObject(nameof(BattleManager)).AddComponent<BattleManager>();
        }

        var playerParticipant = new BattleParticipant(
            participantId: $"fleet-{playerFleet.FleetID}",
            controller: ControllerKind.Player,
            fleet: playerFleet);

        var enemyParticipant = new BattleParticipant(
            participantId: $"fleet-{oppositeFleet.FleetID}",
            controller: ControllerKind.AI,
            fleet: oppositeFleet);

        var session = new BattleSession(
            playerParticipant,
            enemyParticipant,
            ResolutionMode.Playable,
            SceneManager.GetActiveScene().name);

        BattleManager.Instance.StartBattle(session);
    }
}
