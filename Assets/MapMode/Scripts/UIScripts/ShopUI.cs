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
    private string ogPriceSell;


    [Header("Buy Panel Objects")]
    public Transform scrollViewContent;
    public GameObject buyInfoPanel;
    public TMP_Text playerMoney;
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
    public TMP_InputField itemSellAmountInputField;
    public TMP_Text sellPriceText;
    public Button sellBtn;
    public Image itemInfoImageSell;
    public TMP_Text itemDescriptionSell;
    public TMP_Text itemNameSell;
    [Header("Inventory")]
    public Image inventoryIconPrefab;
    public Text inventoryItemNamePrefab;
    public Text inventoryNumberPrefab;
    public Sprite[] inventoryIcons;
    public Transform inventoryScrollViewContent;
    [Header("Choice Buttons")]
    public Button buyButton;
    public Button sellButton;
    private void Start()
    {
        fleet = GameObject.Find("PlayerBoat").GetComponent<PlayerFleetMapController>().GetFleet();
        displayBuyTab(PlayerFleetMapController.currentTown);
        purchaseBtn.interactable = false;
        sellBtn.interactable=false;
        playerMoney.text = PlayerGlobal.money.ToString();
        fillInventoryBuyPage();
    }
    private void Update()
    {
        playerMoney.text = PlayerGlobal.money.ToString();
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

            if (i == 0)
            {
                itemButton.onClick.Invoke();
            }

        }

    }
    public void displayBuyTabBtn()
    {
        foreach (Transform contentChild in scrollViewContent.transform)
        {
            Destroy(contentChild.gameObject);
        }


        buyMode.SetActive(true);
        sellMode.SetActive(false);


        town = PlayerFleetMapController.currentTown;
        string townName = town.name;
        (string[], int[], int[], float[]) itemInfo = town.SupplyDemandPrice();

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

            buyPanel.transform.SetParent(scrollViewContent, false);
            if(i == 0)
            {
                itemButton.onClick.Invoke();
            }

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

        for (int i = 0; i < itemInfo.Item1.Length; i++)
        {
            GameObject buyPanel = Instantiate(buyInfoPanel);
            Button itemButton = buyPanel.GetComponent<Button>();
            itemButton.onClick.AddListener(() => { displayItemInfoSell(itemButton); });

            buyPanel.transform.GetChild(0).GetComponent<Image>().sprite = town.setupSupplyIcons[i];
            buyPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = itemInfo.Item1[i];
            buyPanel.transform.GetChild(2).GetComponent<TMP_Text>().text = itemInfo.Item2[i].ToString();
            buyPanel.transform.GetChild(3).GetComponent<TMP_Text>().text = itemInfo.Item3[i].ToString();
            buyPanel.transform.GetChild(4).GetComponent<TMP_Text>().text = itemInfo.Item4[i].ToString();

            buyPanel.transform.SetParent(scrollViewContentSell, false);

            if (i == 0)
            {
                itemButton.onClick.Invoke();
            }
        }

    }

    public void increaseItemAmount()
    {
        int inputAmount;
        if(buyMode.activeSelf == true)
        {
            inputAmount = int.Parse(itemAmountInputField.text) + 1;
            itemAmountInputField.text = inputAmount.ToString();
        }
        if(sellMode.activeSelf == true)
        {
            inputAmount = int.Parse(itemSellAmountInputField.text) + 1;
            itemSellAmountInputField.text = inputAmount.ToString();
        }

        
    }

    public void decreaseItemAmount()
    {
        int inputAmount;

        if(int.Parse(itemAmountInputField.text) > 0)
        {
            if (buyMode.activeSelf == true)
            {
                inputAmount = int.Parse(itemAmountInputField.text) - 1;
                itemAmountInputField.text = inputAmount.ToString();
            }
            if (sellMode.activeSelf == true)
            {
                inputAmount = int.Parse(itemSellAmountInputField.text) - 1;
                itemSellAmountInputField.text = inputAmount.ToString();
            }
        }
    }

    public void multiplyPrice()
    {
        float multiplePrice;
        if(itemAmountInputField.text != "")
        {
            multiplePrice = int.Parse(itemAmountInputField.text) * float.Parse(ogPrice);
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

    }

    public void multiplySellPrice()
    {
        float multiplePrice;
        if(itemSellAmountInputField.text != "")
        {
            multiplePrice = int.Parse(itemSellAmountInputField.text) * float.Parse(ogPriceSell);
            sellPriceText.text = multiplePrice.ToString();

            (string[], int[]) inventoryContent = fleet.GetInventory();
            for(int i =0; i<inventoryContent.Item1.Length; i++)
            {
                if (inventoryContent.Item1[i].ToLower().Equals(itemNameSell.text.ToLower())){
                    if(int.Parse(itemSellAmountInputField.text) > inventoryContent.Item2[i])
                    {
                        sellBtn.interactable = false;
                    }
                    else
                    {
                        sellBtn.interactable = true;
                    }
                }
            }
        }

    }

    public void displayItemInfo(Button itemPanel)
    {
        itemAmountInputField.text = "";

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

    public void displayItemInfoSell(Button itemPanel)
    {
        itemSellAmountInputField.text = "";

        ogPriceSell = itemPanel.transform.GetChild(4).GetComponent<TMP_Text>().text;
        sellPriceText.text = itemPanel.transform.GetChild(4).GetComponent<TMP_Text>().text;
        itemInfoImageSell.sprite = itemPanel.transform.GetChild(0).GetComponent<Image>().sprite;
        itemNameSell.text = itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text;

        if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "fish")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[0];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "lumber")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[1];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "fur")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[2];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "guns")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[3];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "sugar")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[4];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "coffee")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[5];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "salt")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[6];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "tea")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[7];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "tobacco")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[8];
        }
        else if (itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text == "cotton")
        {
            itemDescriptionSell.text = PlayerFleetMapController.currentTown.setupSupplyDescription[9];
        }
    }

    public void purchaseItem()
    {
        int itemAmount = int.Parse(itemAmountInputField.text);
        float price = float.Parse(priceText.text);

        PlayerFleetMapController.currentTown.FillCargoPlayer(fleet, itemName.text, itemAmount);
        PlayerGlobal.money -= price;
        fillInventoryBuyPage();
    }

    public void sellItem()
    {
        int itemAmount = int.Parse(itemSellAmountInputField.text);
        float price = float.Parse(sellPriceText.text);

        PlayerFleetMapController.currentTown.SellItemsInCargo(fleet, itemAmount, itemNameSell.text);
        PlayerGlobal.money += price;
        displaySellTab();
        fillInventoryBuyPage();
    }

    public void fillInventoryBuyPage()
    {
        (string[], int[]) inventoryContent = fleet.GetInventory();

        foreach (Transform contentChild in inventoryScrollViewContent.transform)
        {
            Destroy(contentChild.gameObject);
        }

        for (int i = 0; i < inventoryContent.Item1.Length; i++)
        {

           
            
                Image icon = Instantiate(inventoryIconPrefab);
                Text inventoryItem = Instantiate(inventoryItemNamePrefab);
                Text inventoryAmount = Instantiate(inventoryNumberPrefab);

                icon.sprite = inventoryIcons[i];
                inventoryItem.text = inventoryContent.Item1[i];
                inventoryAmount.text = inventoryContent.Item2[i].ToString();

                icon.transform.SetParent(inventoryScrollViewContent);
                inventoryItem.transform.SetParent(inventoryScrollViewContent);
                inventoryAmount.transform.SetParent(inventoryScrollViewContent);
            


        }
    }

    public void makeBuyButtonDarkBrown()
    {
        Color darkbrown = new Color(.58f, .47f, .34f, 1.0f);
        Color lightbrown = new Color(.72f, .57f, .4f, 1.0f);

        buyButton.GetComponent<Image>().color = darkbrown;
        sellButton.GetComponent<Image>().color = lightbrown;
    }

    public void makeSellButtonDarkBrown()
    {
        Color darkbrown = new Color(.58f, .47f, .34f, 1.0f);
        Color lightbrown = new Color(.72f, .57f, .4f, 1.0f);

        buyButton.GetComponent<Image>().color = lightbrown;
        sellButton.GetComponent<Image>().color = darkbrown;
    }

}
