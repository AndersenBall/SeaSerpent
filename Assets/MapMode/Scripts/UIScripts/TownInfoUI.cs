using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownInfoUI : MonoBehaviour
{
    private Town town;
    private Fleet fleet;
    private TMP_Text townNameUI;
    private TMP_Text townDescriptionUI;
    private Image townImageUI;

    private TMP_Text resourcePrice;
    private TMP_Text resourceName;
    private GameObject container;


    private void Start()
    {
        townNameUI = GameObject.Find("TownName").GetComponent<TMP_Text>();
        townImageUI = GameObject.Find("TownImage").GetComponent<Image>();
        townDescriptionUI = GameObject.Find("TownDescription").GetComponent<TMP_Text>();
        resourcePrice = GameObject.Find("ResourcePrice").GetComponent<TMP_Text>();
        resourceName = GameObject.Find("ResourceName").GetComponent <TMP_Text>();
        container = GameObject.Find("TownOverviewPanel").gameObject;
        container.SetActive(false);

    }
    public void DisplayTownUI(Town t)
    {
        town = t;

        string townName = t.name;
        string townDescription = t.townDescription;
        Sprite townPic = t.townIcon;

        (string[], int[], int[], float[]) displayInfo = t.SupplyDemandPrice();

        int maxPrice = displayInfo.Item2.Max();
        int maxPriceIndex = Array.IndexOf(displayInfo.Item2, maxPrice);
        string maxPriceItem = displayInfo.Item1[maxPriceIndex];

        townNameUI.text = townName;
        townDescriptionUI.text = townDescription;
        townImageUI.sprite = townPic;
        resourcePrice.text = maxPrice.ToString();
        resourceName.text = maxPriceItem.ToString();

        container.SetActive(true);
    }

    public void CloseTownUI()
    {
        container.SetActive(false);
    }
}