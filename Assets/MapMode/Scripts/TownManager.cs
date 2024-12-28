using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Town[] towns { get; set; }
    public IDictionary<string, float> standardPrices = new Dictionary<string,float>();
    void Start()
    {
        towns= GetComponentsInChildren<Town>();
        standardPrices.Add("fish", 50);
        standardPrices.Add("lumber", 100);
        standardPrices.Add("fur", 2000);
        standardPrices.Add("guns", 2500);
        standardPrices.Add("sugar", 100);
        standardPrices.Add("coffee", 180);
        standardPrices.Add("salt", 70);
        standardPrices.Add("tea", 200);
        standardPrices.Add("tobacco", 400);
        standardPrices.Add("cotton", 150);
       
    }


    public (Fleet, int) RequestItemNonSurplus(string item, int amount, Town originTown)
    {
        // Validate input
        if (originTown == null)
        {
            throw new ArgumentNullException(nameof(originTown), "Origin town cannot be null.");
        }

        float highestProfit = 0;
        int lessAmount = int.MaxValue; // Using int.MaxValue for clarity
        Town chosenTown = null;

        // Iterate through all towns to find the most profitable trade route
        foreach (Town t in towns)
        {
            // Skip the origin town (no trading with itself)
            if (t.name == originTown.name)
            {
                continue; // Use continue instead of break to avoid exiting the loop entirely
            }

            // Determine the amount of goods to trade
            int equalAmount = BlanceResourceAmount(t, originTown, item);

            // Skip towns where no tradeable amount is available
            if (equalAmount == 0)
            {
                continue;
            }

            // Calculate the cost of the journey between towns
            float transportationCost = JourneyCost(t, originTown, equalAmount);

            // Calculate the profit from goods alone (revenue - cost of acquisition)
            float profitGoods = originTown.CalculateTransactionPrice(item, equalAmount)
                              - t.CalculateTransactionPrice(item, -equalAmount);

            // Total profit after including transportation cost
            float totalProfit = profitGoods - transportationCost;

            // Check if this trade is the most profitable and meets the minimum profit threshold
            if (totalProfit > highestProfit && totalProfit > 5000)
            {
                highestProfit = totalProfit;
                chosenTown = t;
                lessAmount = equalAmount;
            }
        }


        // Adjust the amount to be less if it was more profittable to not fill the entire order
        amount = Mathf.Min(amount, lessAmount);

        // If a viable trade route is found
        if (chosenTown != null)
        {
            Fleet fleet = chosenTown.MakeTradeFleet(item, amount);
            if (fleet == null)
            {
                Debug.LogError($"Failed to create a fleet for item: {item}, amount: {amount}.");
                return (null, -1);
            }

            chosenTown.SendOutFleet(fleet, originTown.transform);
            return (fleet, amount);
        }

        // No viable trade route found
        Debug.Log($"No viable trade route found for item: {item} from origin town: {originTown.name}.");
        return (null, -1);
    }

    //amount of a resource to transfer between the 2 towns
    public int BlanceResourceAmount(Town t1, Town t2, string r) {//amount to remove from t1 -> t2 to balance
        int amount = ((t2.DemandOfItem(r) * t1.SupplyOfItem(r)) - (t1.DemandOfItem(r) * t2.SupplyOfItem(r))) 
            / (t2.DemandOfItem(r) + t1.DemandOfItem(r));

        //Debug.Log(t1.name + t2.name + "amount to send, to equal is:" + amount + " d1:"+t1.DemandOfItem(r)+ " d2:" + t2.DemandOfItem(r) + " s1:" + t1.SupplyOfItem(r) + " s2:" + t2.SupplyOfItem(r));
        return Mathf.Max(amount, 0);
    }

    public float JourneyCost(Town t1, Town t2,int amount) {
        int numberOfShips = Mathf.CeilToInt((float)amount / 100);
        //Debug.Log(amount+", " + numberOfShips + "price of journey:" +t1.name +"->"+ t2.name + (numberOfShips * (t1.transform.position - t2.transform.position).magnitude * 3 + 100));
        return numberOfShips * (t1.transform.position-t2.transform.position).magnitude*5+100;
    }
}
