using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    [System.Serializable]
    public struct MoneyData
    {
        public float money; 
    }

    private void Awake()
    {

        GameEvents.SaveInitiated += SaveMoney;
        GameEvents.LoadInitiated += LoadMoney;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the save and load events to prevent memory leaks
        GameEvents.SaveInitiated -= SaveMoney;
        GameEvents.LoadInitiated -= LoadMoney;
    }

    private void SaveMoney()
    {
        MoneyData moneyData = new MoneyData
        {
            money = PlayerGlobal.money
        };

        SaveLoad.Save(moneyData, "Money");
        Debug.Log($"Money saved successfully: {PlayerGlobal.money}");
    }


    private void LoadMoney()
    {
        if (SaveLoad.SaveExists("Money"))
        {
            MoneyData moneyData = SaveLoad.Load<MoneyData>("Money");
            PlayerGlobal.money = moneyData.money;
            Debug.Log($"Money loaded successfully: {PlayerGlobal.money}");
        }
        else
        {
            Debug.Log("No money save data found. Setting default value to 1000000.");
            PlayerGlobal.money = 500000;
        }
    }
}
