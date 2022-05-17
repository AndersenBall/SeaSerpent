using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventoryUI : MonoBehaviour
{
    Fleet fleet;

    private Transform inventoryUI;
    private Transform inventoryContainer;
    private Transform entryTemplate;
    string state = "none";

    private List<Transform> transformList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI = transform.Find("InventoryUI");
        inventoryContainer = inventoryUI.Find("InventoryUIContainer");
        entryTemplate = inventoryContainer.transform.Find("ColExample");
        entryTemplate.gameObject.SetActive(false);

        fleet = GameObject.Find("PlayerBoat").GetComponent<PlayerFleetMapController>().GetFleet();
         
    }
    // Update is called once per frame
 
    public void DisplayInventoryUI()
    {
        fleet = GameObject.Find("PlayerBoat").GetComponent<PlayerFleetMapController>().GetFleet();
        Debug.Log("open inventory"+fleet.FleetID);
        string commandName = fleet.commander;
        (string[], int[]) displayInfo = fleet.GetInventory();

        inventoryUI.gameObject.SetActive(true);

        Transform commanderText = transform.Find("InventoryUI/CommanderName");
        commanderText.GetComponent<Text>().text = "Commander:" + commandName;
        transform.Find("InventoryUI/Money").GetComponent<Text>().text = "money:" + PlayerGlobal.money;

        float templateHeight = 30f;

        for (int i = 0; i < displayInfo.Item1.Length; i++) {
            Transform entryTransform = Instantiate(entryTemplate, inventoryContainer);
            transformList.Add(entryTransform);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * (i + 1));
            entryTransform.gameObject.SetActive(true);

            entryTransform.Find("item").GetComponent<Text>().text = "" + displayInfo.Item1[i];
            entryTransform.Find("suply").GetComponent<Text>().text = "" + displayInfo.Item2[i];
           

        }
    }

    public void CloseInventoryUI() {
        Debug.Log("close inventory");
        foreach (Transform t in transformList) {
            GameObject.Destroy(t.gameObject);
        }
        transformList = new List<Transform>();
        inventoryUI.gameObject.SetActive(false);
    }
    public void ChangeState(string s) {
        if (s == "None") {
            CloseInventoryUI();
        }
        if (s == "Inventory") {
            DisplayInventoryUI();
        }
        state = s;
    }
    public string GetState() { return state; }
}
