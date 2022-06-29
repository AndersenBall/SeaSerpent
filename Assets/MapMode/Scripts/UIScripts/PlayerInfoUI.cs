using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    public Sprite[] inventoryIcons;

    public Transform scrollViewContent;
    public Image inventoryIconPrefab;
    public Text inventoryItemNamePrefab;
    public Text inventoryNumberPrefab;



    private Fleet fleet;
    private TMP_Text playerNameUI;
    private TMP_Text moneyUI;
    
    private string playerName;
    private string playerMoney;
    
    void Start()
    {
        fleet = GameObject.Find("PlayerBoat").GetComponent<PlayerFleetMapController>().GetFleet();
        playerNameUI = GameObject.Find("PlayerName").GetComponent<TMP_Text>();
        moneyUI = GameObject.Find("Money").GetComponent<TMP_Text>();
        setUserInformation();
        fillInventory();
    }

    public void setUserInformation()
    {
        playerName = fleet.commander;
        playerNameUI.text = "  Commander " + playerName;

        playerMoney = PlayerGlobal.money.ToString();
        moneyUI.text = "$" + playerMoney;
    }

    public void fillInventory()
    {
        (string[], int[]) inventoryContent = fleet.GetInventory();
        for (int i = 0; i < inventoryContent.Item1.Length; i++)
        {
            Image icon = Instantiate(inventoryIconPrefab);
            Text inventoryItem = Instantiate(inventoryItemNamePrefab);
            Text inventoryAmount = Instantiate(inventoryNumberPrefab);

            icon.sprite = inventoryIcons[i];
            inventoryItem.text = inventoryContent.Item1[i];
            inventoryAmount.text = inventoryContent.Item2[i].ToString();

            icon.transform.SetParent(scrollViewContent);
            inventoryItem.transform.SetParent(scrollViewContent);
            inventoryAmount.transform.SetParent(scrollViewContent);


        }
    }
}
