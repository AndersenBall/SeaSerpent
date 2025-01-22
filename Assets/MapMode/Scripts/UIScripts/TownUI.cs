using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TownUI : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;
    private Transform shopUI;

    private Transform quanityContainerBuy;
    private Transform quanityTemplateBuy;

    private Transform quanityContainerSell;
    private Transform quanityTemplateSell;

    private Town town;
    private Fleet fleet;
    private List<Transform> transformList = new List<Transform>();

    private int amount = 50;
    

    private void Start(){
        entryContainer = transform.Find("ShopUI/townUiContainer");
        entryTemplate = entryContainer.transform.Find("ColExample");
        entryTemplate.gameObject.SetActive(false);

        quanityContainerBuy = transform.Find("QuantityBuy");
        quanityTemplateBuy = quanityContainerBuy.Find("QuantityEx");
        quanityContainerSell = transform.Find("QuantitySell");
        quanityTemplateSell = quanityContainerSell.Find("QuantityEx");


        shopUI = transform.Find("ShopUI");
    }
        
    
    public void DisplayTownUI(Town t, Fleet f) {
        Time.timeScale = .000f;

        string townName = t.name;
        fleet = f;
        town = t;
        (string[], int[], int[], float[]) displayInfo = t.SupplyDemandPrice();

        shopUI.gameObject.SetActive(true);

        Transform townText = transform.Find("ShopUI/Town");
        townText.GetComponent<Text>().text = "Town: " + townName;

        float templateHeight = 30f;

        for (int i = 0; i < displayInfo.Item1.Length; i++) {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            transformList.Add(entryTransform);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * (i+1));
            entryTransform.gameObject.SetActive(true);

            entryTransform.Find("item").GetComponent<Text>().text = ""+displayInfo.Item1[i];
            entryTransform.Find("suply").GetComponent<Text>().text = ""+ displayInfo.Item2[i];
            entryTransform.Find("demand").GetComponent<Text>().text = ""+ displayInfo.Item3[i];
            entryTransform.Find("price").GetComponent<Text>().text = ""+ displayInfo.Item4[i];
         
            string r = displayInfo.Item1[i];
            entryTransform.Find("buttons/Buy").GetComponent<Button>().onClick.AddListener(delegate { OpenAmountUI(r); });
            entryTransform.Find("buttons/Sell").GetComponent<Button>().onClick.AddListener(delegate { OpenSellAmountUI(r); });
        }
    
    }
    public void CloseTownUI() {
        foreach (Transform t in transformList) {
            GameObject.Destroy(t.gameObject);
        }
        transformList = new List<Transform>();
        shopUI.gameObject.SetActive(false);
        quanityContainerBuy.gameObject.SetActive(false);
        quanityContainerSell.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenAmountUI(string resource) {
        quanityContainerBuy.gameObject.SetActive(true);
        quanityTemplateBuy.Find("Buy").GetComponent<Button>().onClick.RemoveAllListeners();
        quanityTemplateBuy.Find("Buy").GetComponent<Button>().onClick.AddListener(delegate { BuyItems(resource); });
        amount = 50;
        quanityTemplateBuy.Find("Amount").GetComponent<Text>().text = "" + amount;
    }

    public void BuyItems(string resource) {
        town.FillCargoPlayer(fleet,resource,amount);
        CloseTownUI();
        DisplayTownUI(town,fleet);
        
    }

    public void ChangeCount(int a) {
        amount += a;
        quanityTemplateBuy.Find("Amount").GetComponent<Text>().text = ""+amount;
        quanityTemplateSell.Find("Amount").GetComponent<Text>().text = "" + amount;
    }
    public void OpenSellAmountUI(string resource) {
        quanityContainerSell.gameObject.SetActive(true);
        quanityTemplateSell.transform.Find("Sell").GetComponent<Button>().onClick.RemoveAllListeners();
        quanityTemplateSell.transform.Find("Sell").GetComponent<Button>().onClick.AddListener(delegate { SellItem(resource); });
        amount = 50;
        quanityTemplateSell.transform.Find("Amount").GetComponent<Text>().text = "" + amount;

    }

    public void SellItem(string resource) {//player sell item
        town.SellItemsInCargo(fleet,amount,resource);
        CloseTownUI();
        DisplayTownUI(town, fleet);
    }

    public void LoadSceneBoatShop()
    {
        SceneTransfer.TransferToTownUI();
        
        
    }

    

}
