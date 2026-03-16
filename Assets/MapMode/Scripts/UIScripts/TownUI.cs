using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform entryContainer;
    [SerializeField] private Transform entryTemplate;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private Text townLabel;
    [SerializeField] private Transform quantityContainerBuy;
    [SerializeField] private Transform quantityTemplateBuy;
    [SerializeField] private Transform quantityContainerSell;
    [SerializeField] private Transform quantityTemplateSell;

    private Town town;
    private Fleet fleet;
    private readonly List<Transform> rowInstances = new List<Transform>();
    private int amount = 50;

    private void Awake()
    {
        entryTemplate.gameObject.SetActive(false);
    }

    public void DisplayTownUI(Town selectedTown, Fleet selectedFleet)
    {
        Time.timeScale = 0f;

        fleet = selectedFleet;
        town = selectedTown;
        (string[] itemNames, int[] supply, int[] demand, float[] prices) = selectedTown.SupplyDemandPrice();

        shopUI.SetActive(true);
        townLabel.text = "Town: " + selectedTown.name;

        float templateHeight = 30f;
        for (int i = 0; i < itemNames.Length; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            rowInstances.Add(entryTransform);

            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * (i + 1));
            entryTransform.gameObject.SetActive(true);

            entryTransform.Find("item").GetComponent<Text>().text = itemNames[i];
            entryTransform.Find("suply").GetComponent<Text>().text = supply[i].ToString();
            entryTransform.Find("demand").GetComponent<Text>().text = demand[i].ToString();
            entryTransform.Find("price").GetComponent<Text>().text = prices[i].ToString();

            string resource = itemNames[i];
            entryTransform.Find("buttons/Buy").GetComponent<Button>().onClick.AddListener(() => OpenAmountUI(resource));
            entryTransform.Find("buttons/Sell").GetComponent<Button>().onClick.AddListener(() => OpenSellAmountUI(resource));
        }
    }

    public void CloseTownUI()
    {
        foreach (Transform row in rowInstances)
        {
            Destroy(row.gameObject);
        }

        rowInstances.Clear();
        shopUI.SetActive(false);
        quantityContainerBuy.gameObject.SetActive(false);
        quantityContainerSell.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenAmountUI(string resource)
    {
        quantityContainerBuy.gameObject.SetActive(true);
        Button buyButton = quantityTemplateBuy.Find("Buy").GetComponent<Button>();
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyItems(resource));

        amount = 50;
        quantityTemplateBuy.Find("Amount").GetComponent<Text>().text = amount.ToString();
    }

    public void BuyItems(string resource)
    {
        town.FillCargoPlayer(fleet, resource, amount);
        CloseTownUI();
        DisplayTownUI(town, fleet);
    }

    public void ChangeCount(int change)
    {
        amount += change;
        quantityTemplateBuy.Find("Amount").GetComponent<Text>().text = amount.ToString();
        quantityTemplateSell.Find("Amount").GetComponent<Text>().text = amount.ToString();
    }

    public void OpenSellAmountUI(string resource)
    {
        quantityContainerSell.gameObject.SetActive(true);
        Button sellButton = quantityTemplateSell.Find("Sell").GetComponent<Button>();
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => SellItem(resource));

        amount = 50;
        quantityTemplateSell.Find("Amount").GetComponent<Text>().text = amount.ToString();
    }

    public void SellItem(string resource)
    {
        town.SellItemsInCargo(fleet, amount, resource);
        CloseTownUI();
        DisplayTownUI(town, fleet);
    }

    public void LoadSceneBoatShop()
    {
        SceneTransfer.TransferToTownUI();
    }
}
