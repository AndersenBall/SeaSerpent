using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private Town town;
    private Fleet fleet;
    private string ogPrice;

    [Header("Buy Panel Objects")]
    public Transform scrollViewContent;
    public GameObject buyInfoPanel;
    public GameObject playerMoney;
    public TMP_Text priceText;
    public TMP_InputField itemAmountInputField;
    public Image itemInfoImage;
    public TMP_Text itemDescription;
    public TMP_Text itemName;
    public Button purchaseBtn;
    public GameObject buyMode;
    [Header("Sell Panel Objects")]
    public Transform scrollViewContentSell;
    public GameObject sellMode;

    private void Start()
    {
        fleet = GameObject.Find("PlayerBoat").GetComponent<PlayerFleetMapController>().GetFleet();
        displayBuyTab(PlayerFleetMapController.currentTown);
    }


    public void displayBuyTab(Town t)
    {

        foreach (Transform contentChild in scrollViewContent.transform)
        {
            Destroy(contentChild.gameObject);
        }


        buyMode.SetActive(true);
        sellMode.SetActive(false);

        string townName = t.name;
        town = t;
        (string[], int[], int[], float[]) itemInfo = t.SupplyDemandPrice();
        Debug.Log(townName);

        for (int i = 0; i < itemInfo.Item1.Length; i++)
        {
            GameObject buyPanel = Instantiate(buyInfoPanel);
            Button itemButton = buyPanel.GetComponent<Button>();
            itemButton.onClick.AddListener(() => { displayItemInfo(itemButton);});

            buyPanel.transform.GetChild(0).GetComponent<Image>().sprite = t.setupSupplyIcons[i];
             buyPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = itemInfo.Item1[i];
             buyPanel.transform.GetChild(2).GetComponent<TMP_Text>().text = itemInfo.Item2[i].ToString();
             buyPanel.transform.GetChild(3).GetComponent<TMP_Text>().text = itemInfo.Item3[i].ToString();
             buyPanel.transform.GetChild(4).GetComponent<TMP_Text>().text = itemInfo.Item4[i].ToString();

            buyPanel.transform.SetParent(scrollViewContent, false);

        }

    }
    public void displaySellTab()
    {
        foreach (Transform contentChild in scrollViewContentSell.transform)
        {
            Destroy(contentChild.gameObject);
        }

        buyMode.SetActive(false);
        sellMode.SetActive(true);

        town = PlayerFleetMapController.currentTown;
        string townName = town.name;
        (string[], int[], int[], float[]) itemInfo = town.SupplyDemandPrice();
        Debug.Log(townName);

        for (int i = 0; i < itemInfo.Item1.Length; i++)
        {
            GameObject buyPanel = Instantiate(buyInfoPanel);
            Button itemButton = buyPanel.GetComponent<Button>();
            itemButton.onClick.AddListener(() => { displayItemInfo(itemButton); });

            buyPanel.transform.GetChild(0).GetComponent<Image>().sprite = town.setupSupplyIcons[i];
            buyPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = itemInfo.Item1[i];
            buyPanel.transform.GetChild(2).GetComponent<TMP_Text>().text = itemInfo.Item2[i].ToString();
            buyPanel.transform.GetChild(3).GetComponent<TMP_Text>().text = itemInfo.Item3[i].ToString();
            buyPanel.transform.GetChild(4).GetComponent<TMP_Text>().text = itemInfo.Item4[i].ToString();

            buyPanel.transform.SetParent(scrollViewContentSell, false);

        }

    }

    public void increaseItemAmount()
    {
        int inputAmount = int.Parse(itemAmountInputField.text) + 1;

        itemAmountInputField.text = inputAmount.ToString();
    }

    public void decreaseItemAmount()
    {
        int inputAmount = int.Parse(itemAmountInputField.text) - 1;

        if(int.Parse(itemAmountInputField.text) > 0)
        {
            itemAmountInputField.text = inputAmount.ToString();
        }
    }

    public void multiplyPrice()
    {
        int multiplePrice = int.Parse(itemAmountInputField.text) * int.Parse(ogPrice);
        priceText.text = multiplePrice.ToString();

        if ((PlayerGlobal.money - multiplePrice) >= 0)
        {
            purchaseBtn.interactable = true;
        }
        else
        {
            purchaseBtn.interactable = false;
        }
    }

    public void displayItemInfo(Button itemPanel)
    {
        ogPrice = itemPanel.transform.GetChild(4).GetComponent<TMP_Text>().text;
        priceText.text = itemPanel.transform.GetChild(4).GetComponent<TMP_Text>().text;
        itemInfoImage.sprite = itemPanel.transform.GetChild(0).GetComponent<Image>().sprite;
        itemName.text = itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text;

        if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "fish")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[0];
        }else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "lumber")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[1];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "fur")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[2];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "guns")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[3];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "sugar")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[4];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "coffee")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[5];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "salt")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[6];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "tea")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[7];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "tobacco")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[8];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "cotton")
        {
            itemDescription.text = PlayerFleetMapController.currentTown.setupSupplyDescription[9];
        }
    }

    public void purchaseItem()
    {
        int itemAmount = int.Parse(itemAmountInputField.text);
        int price = int.Parse(priceText.text);

        PlayerFleetMapController.currentTown.FillCargoPlayer(fleet, itemName.text, itemAmount);
        PlayerGlobal.money -= price;
        displayBuyTab(PlayerFleetMapController.currentTown);

    }


}
