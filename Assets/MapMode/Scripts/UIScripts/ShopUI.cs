using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private Town town;
    private Fleet fleet;

    public Transform scrollViewContent;
    public GameObject buyInfoPanel;

    private void Start()
    {
        displayBuyTab(PlayerFleetMapController.currentTown);
    }


    private void displayBuyTab(Town t)
    {
        string townName = t.name;
        town = t;
        (string[], int[], int[], float[]) itemInfo = t.SupplyDemandPrice();
        Debug.Log(townName);

        for (int i = 0; i < itemInfo.Item1.Length; i++)
        {
            GameObject buyPanel = Instantiate(buyInfoPanel);

            buyPanel.transform.GetChild(0).GetComponent<Image>().sprite = t.setupSupplyIcons[i];
             buyPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = itemInfo.Item1[i];
             buyPanel.transform.GetChild(2).GetComponent<TMP_Text>().text = itemInfo.Item2[i].ToString();
             buyPanel.transform.GetChild(3).GetComponent<TMP_Text>().text = itemInfo.Item3[i].ToString();
             buyPanel.transform.GetChild(4).GetComponent<TMP_Text>().text = itemInfo.Item4[i].ToString();

            buyPanel.transform.SetParent(scrollViewContent, false);

        }

    }
}
