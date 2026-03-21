using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("Context")]
    [SerializeField] private TownUIContext townUIContext;

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

    private void OnEnable()
    {
        if (townUIContext != null)
        {
            townUIContext.ContextChanged += HandleContextChanged;
        }
    }

    private void OnDisable()
    {
        if (townUIContext != null)
        {
            townUIContext.ContextChanged -= HandleContextChanged;
        }
    }

    private void Start()
    {
        purchaseBtn.interactable = false;
        sellBtn.interactable = false;

        if (townUIContext == null)
        {
            Debug.LogError($"[{nameof(ShopUI)}] Missing {nameof(TownUIContext)} reference.");
            enabled = false;
            return;
        }

        HandleContextChanged(townUIContext.CurrentTown, townUIContext.CurrentFleet);
        playerMoney.text = PlayerStateService.Money.ToString();
    }

    private void Update()
    {
        playerMoney.text = PlayerStateService.Money.ToString();
    }

    private void HandleContextChanged(Town newTown, Fleet newFleet)
    {
        town = newTown;
        fleet = newFleet;

        if (town == null || fleet == null)
        {
            return;
        }

        displayBuyTab(town);
        fillInventoryBuyPage();
    }

    public void displayBuyTab(Town selectedTown)
    {
        if (selectedTown == null)
        {
            return;
        }

        town = selectedTown;
        PopulateTradeList(scrollViewContent, true);
    }

    public void displayBuyTabBtn()
    {
        if (town == null)
        {
            return;
        }

        PopulateTradeList(scrollViewContent, true);
    }

    public void displaySellTab()
    {
        if (town == null)
        {
            return;
        }

        PopulateTradeList(scrollViewContentSell, false);
    }

    private void PopulateTradeList(Transform container, bool isBuy)
    {
        foreach (Transform contentChild in container)
        {
            Destroy(contentChild.gameObject);
        }

        buyMode.SetActive(isBuy);
        sellMode.SetActive(!isBuy);

        (string[] itemNames, int[] supply, int[] demand, float[] prices) = town.SupplyDemandPrice();

        for (int i = 0; i < itemNames.Length; i++)
        {
            GameObject panel = Instantiate(buyInfoPanel, container, false);
            Button itemButton = panel.GetComponent<Button>();

            int index = i;
            if (isBuy)
            {
                itemButton.onClick.AddListener(() => displayItemInfo(itemButton));
            }
            else
            {
                itemButton.onClick.AddListener(() => displayItemInfoSell(itemButton));
            }

            panel.transform.GetChild(0).GetComponent<Image>().sprite = town.setupSupplyIcons[index];
            panel.transform.GetChild(1).GetComponent<TMP_Text>().text = itemNames[index];
            panel.transform.GetChild(2).GetComponent<TMP_Text>().text = supply[index].ToString();
            panel.transform.GetChild(3).GetComponent<TMP_Text>().text = demand[index].ToString();
            panel.transform.GetChild(4).GetComponent<TMP_Text>().text = prices[index].ToString();

            if (i == 0)
            {
                itemButton.onClick.Invoke();
            }
        }
    }

    public void increaseItemAmount()
    {
        if (buyMode.activeSelf)
        {
            itemAmountInputField.text = (int.Parse(itemAmountInputField.text) + 1).ToString();
        }

        if (sellMode.activeSelf)
        {
            itemSellAmountInputField.text = (int.Parse(itemSellAmountInputField.text) + 1).ToString();
        }
    }

    public void decreaseItemAmount()
    {
        if (buyMode.activeSelf && int.Parse(itemAmountInputField.text) > 0)
        {
            itemAmountInputField.text = (int.Parse(itemAmountInputField.text) - 1).ToString();
        }

        if (sellMode.activeSelf && int.Parse(itemSellAmountInputField.text) > 0)
        {
            itemSellAmountInputField.text = (int.Parse(itemSellAmountInputField.text) - 1).ToString();
        }
    }

    public void multiplyPrice()
    {
        if (itemAmountInputField.text == "")
        {
            return;
        }

        float multiplePrice = int.Parse(itemAmountInputField.text) * float.Parse(ogPrice);
        priceText.text = multiplePrice.ToString();
        purchaseBtn.interactable = (PlayerStateService.Money - multiplePrice) >= 0;
    }

    public void multiplySellPrice()
    {
        if (itemSellAmountInputField.text == "")
        {
            return;
        }

        float multiplePrice = int.Parse(itemSellAmountInputField.text) * float.Parse(ogPriceSell);
        sellPriceText.text = multiplePrice.ToString();

        (string[] names, int[] amounts) inventoryContent = fleet.GetInventory();
        for (int i = 0; i < inventoryContent.names.Length; i++)
        {
            if (!inventoryContent.names[i].ToLower().Equals(itemNameSell.text.ToLower()))
            {
                continue;
            }

            sellBtn.interactable = int.Parse(itemSellAmountInputField.text) <= inventoryContent.amounts[i];
            return;
        }

        sellBtn.interactable = false;
    }

    public void displayItemInfo(Button itemPanel)
    {
        itemAmountInputField.text = "";

        ogPrice = itemPanel.transform.GetChild(4).GetComponent<TMP_Text>().text;
        priceText.text = ogPrice;
        itemInfoImage.sprite = itemPanel.transform.GetChild(0).GetComponent<Image>().sprite;

        string selectedItemName = itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text;
        itemName.text = selectedItemName;
        itemDescription.text = GetItemDescription(selectedItemName);
    }

    public void displayItemInfoSell(Button itemPanel)
    {
        itemSellAmountInputField.text = "";

        ogPriceSell = itemPanel.transform.GetChild(4).GetComponent<TMP_Text>().text;
        sellPriceText.text = ogPriceSell;
        itemInfoImageSell.sprite = itemPanel.transform.GetChild(0).GetComponent<Image>().sprite;

        string selectedItemName = itemPanel.transform.GetChild(1).GetComponent<TMP_Text>().text;
        itemNameSell.text = selectedItemName;
        itemDescriptionSell.text = GetItemDescription(selectedItemName);
    }

    public void purchaseItem()
    {
        if (town == null || fleet == null)
        {
            return;
        }

        int itemAmount = int.Parse(itemAmountInputField.text);
        float price = float.Parse(priceText.text);

        town.FillCargoPlayer(fleet, itemName.text, itemAmount);
        if (PlayerStateService.TrySpendMoney(price))
        {
            fillInventoryBuyPage();
        }
    }

    public void sellItem()
    {
        if (town == null || fleet == null)
        {
            return;
        }

        int itemAmount = int.Parse(itemSellAmountInputField.text);
        float price = float.Parse(sellPriceText.text);

        town.SellItemsInCargo(fleet, itemAmount, itemNameSell.text);
        PlayerStateService.AddMoney(price);
        displaySellTab();
        fillInventoryBuyPage();
    }

    public void fillInventoryBuyPage()
    {
        if (fleet == null)
        {
            return;
        }

        (string[] names, int[] amounts) inventoryContent = fleet.GetInventory();

        foreach (Transform contentChild in inventoryScrollViewContent.transform)
        {
            Destroy(contentChild.gameObject);
        }

        for (int i = 0; i < inventoryContent.names.Length; i++)
        {
            Image icon = Instantiate(inventoryIconPrefab, inventoryScrollViewContent, false);
            Text inventoryItem = Instantiate(inventoryItemNamePrefab, inventoryScrollViewContent, false);
            Text inventoryAmount = Instantiate(inventoryNumberPrefab, inventoryScrollViewContent, false);

            icon.sprite = inventoryIcons[i];
            inventoryItem.text = inventoryContent.names[i];
            inventoryAmount.text = inventoryContent.amounts[i].ToString();
        }
    }

    private string GetItemDescription(string itemNameToFind)
    {
        var tradeInfo = town.SupplyDemandPrice();
        string[] itemNames = tradeInfo.Item1;
        for (int i = 0; i < itemNames.Length; i++)
        {
            if (itemNames[i].Equals(itemNameToFind))
            {
                return town.setupSupplyDescription[i];
            }
        }

        return string.Empty;
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
