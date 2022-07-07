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

    private TMP_Text largestDemandPrice;
    private TMP_Text largestSupplyPrice;
    private TMP_Text largestDemandName;
    private TMP_Text largestSupplyName;
    private Image largestDemandIcon;
    private Image largestSupplyIcon;
    private GameObject container;


    private void Start()
    {
        townNameUI = GameObject.Find("TownName").GetComponent<TMP_Text>();
        townImageUI = GameObject.Find("TownImage").GetComponent<Image>();
        townDescriptionUI = GameObject.Find("TownDescription").GetComponent<TMP_Text>();
        largestDemandPrice = GameObject.Find("HighestDemandPrice").GetComponent<TMP_Text>();
        largestSupplyPrice = GameObject.Find("HighestSupplyPrice").GetComponent <TMP_Text>();
        largestSupplyName = GameObject.Find("HighestSupplyName").GetComponent<TMP_Text>();
        largestDemandName = GameObject.Find("HighestDemandName").GetComponent<TMP_Text>();
        largestDemandIcon = GameObject.Find("HighestDemandImage").GetComponent<Image>();
        largestSupplyIcon = GameObject.Find("HighestSupplyImage").GetComponent<Image>();
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

        int maxDemand = displayInfo.Item3.Max();
        int maxDemandIndex = Array.IndexOf(displayInfo.Item3, maxDemand);
        string maxDemandItem = displayInfo.Item1[maxDemandIndex];
        largestDemandName.text = maxDemandItem;

        int maxSupply = displayInfo.Item2.Max();
        int maxSupplyIndex = Array.IndexOf(displayInfo.Item2, maxSupply);
        string maxSupplyItem = displayInfo.Item1[maxSupplyIndex];
        largestSupplyName.text = maxSupplyItem;

        for (int i = 0; i < displayInfo.Item1.Length; i++)
              {
                  if(displayInfo.Item1[i] == maxDemandItem)
                  {
                      largestDemandIcon.sprite = t.setupSupplyIcons[i];
                      largestDemandPrice.text = "$" + displayInfo.Item4[i].ToString();
                  }

                  if(displayInfo.Item1[i] == maxSupplyItem)
                  { 
                      largestSupplyIcon.sprite = t.setupSupplyIcons[i];
                      largestSupplyPrice.text = "$" + displayInfo.Item4[i].ToString();
                  }
             } 

        townNameUI.text = townName;
        townDescriptionUI.text = townDescription;
        townImageUI.sprite = townPic;

        container.SetActive(true);
    }

    public void CloseTownUI()
    {
        container.SetActive(false);
    }
}