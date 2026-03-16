using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownInfoUI : MonoBehaviour
{
    [Header("Overview References")]
    [SerializeField] private TMP_Text townNameUI;
    [SerializeField] private TMP_Text townDescriptionUI;
    [SerializeField] private Image townImageUI;

    [Header("Supply / Demand References")]
    [SerializeField] private TMP_Text largestDemandPrice;
    [SerializeField] private TMP_Text largestSupplyPrice;
    [SerializeField] private TMP_Text largestDemandName;
    [SerializeField] private TMP_Text largestSupplyName;
    [SerializeField] private Image largestDemandIcon;
    [SerializeField] private Image largestSupplyIcon;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void DisplayTownUI(Town town)
    {
        if (town == null)
        {
            return;
        }

        (string[] itemNames, int[] supply, int[] demand, float[] prices) = town.SupplyDemandPrice();

        int maxDemand = demand.Max();
        int maxDemandIndex = Array.IndexOf(demand, maxDemand);
        int maxSupply = supply.Max();
        int maxSupplyIndex = Array.IndexOf(supply, maxSupply);

        largestDemandName.text = itemNames[maxDemandIndex];
        largestSupplyName.text = itemNames[maxSupplyIndex];
        largestDemandIcon.sprite = town.setupSupplyIcons[maxDemandIndex];
        largestSupplyIcon.sprite = town.setupSupplyIcons[maxSupplyIndex];
        largestDemandPrice.text = "$" + prices[maxDemandIndex];
        largestSupplyPrice.text = "$" + prices[maxSupplyIndex];

        townNameUI.text = town.name;
        townDescriptionUI.text = town.townDescription;
        townImageUI.sprite = town.townIcon;

        gameObject.SetActive(true);
    }

    public void CloseTownUI()
    {
        gameObject.SetActive(false);
    }
}
