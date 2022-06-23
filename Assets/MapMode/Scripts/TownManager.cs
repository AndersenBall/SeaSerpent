using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Town[] towns;
    public IDictionary<string, float> standardPrices = new Dictionary<string,float>();
    void Start()
    {
        towns= GetComponentsInChildren<Town>();
        standardPrices.Add("fish", 50);
        standardPrices.Add("lumber", 100);
        standardPrices.Add("fur", 5000);
        standardPrices.Add("guns", 2500);
        standardPrices.Add("sugar", 100);
        standardPrices.Add("coffee", 80);
        standardPrices.Add("salt", 70);
        standardPrices.Add("tea", 1000);
        standardPrices.Add("tobacco", 400);
        standardPrices.Add("cotton", 150);
       
    }

  
    public (Fleet,int) RequestItemNonSurplus(string item, int amount, Town originTown)
    {
        //Debug.Log("amount:" + amount);
        float highestProfit = 0;
        int choosenAmount = 1000000000;
        Town choosenTown = originTown;
        foreach (Town t in towns) {
            
            int equalAmount = BlanceResourceAmount(t, originTown, item);
            float cost = JourneyCost(t, originTown, equalAmount);
            float revenue = originTown.CalculatePrice(standardPrices[item], item, equalAmount) - t.CalculatePrice(standardPrices[item], item, -equalAmount);
            float profit = revenue - cost;// NEED TO WRITE PROFIT using new calculated price of orig vs new town
            Debug.Log("profit: " + profit +" revenue: " + revenue +","+ originTown.CalculatePrice(standardPrices[item], item, equalAmount)+ " cost: " + -cost +"," + -t.CalculatePrice(standardPrices[item], item, -equalAmount) + " amout:"+equalAmount+ " buy from " + t.name+ "-> sell to: " + originTown.name);
            if (profit > highestProfit && profit > 5000) {
                highestProfit = profit;
                choosenTown = t;
                choosenAmount = equalAmount;
            }
        }

        amount =  amount < choosenAmount ? amount : choosenAmount;
        //amount = choosenAmount;
        if (choosenTown != originTown) {
            //Debug.Log(originTown.name + " " + item +","+ amount+" requested from " + choosenTown.name +" Profit:"+highestProfit);
            Fleet f = choosenTown.MakeTradeFleet(item, amount);
            choosenTown.SendOutFleet(f, originTown.transform);
            return (f,amount);
        }
        else {
            //Debug.Log(originTown.name + " " + item + " no request, no viable buyers");
            Fleet f = null;
            return (f,-1);
        }


    }
    public int BlanceResourceAmount(Town t1, Town t2, string r) {//amount to remove from t1 -> t2 to balance
        int amount = ((t2.DemandOfItem(r) * t1.SupplyOfItem(r)) - (t1.DemandOfItem(r) * t2.SupplyOfItem(r))) 
            / (t2.DemandOfItem(r) + t1.DemandOfItem(r));

        //Debug.Log(t1.name + t2.name + "amount to send, to equal is:" + amount + " d1:"+t1.DemandOfItem(r)+ " d2:" + t2.DemandOfItem(r) + " s1:" + t1.SupplyOfItem(r) + " s2:" + t2.SupplyOfItem(r));
        if (amount < 0) {
            amount = 0;
        }
        return amount;
    }

    public float JourneyCost(Town t1, Town t2,int amount) {
        int numberOfShips = Mathf.CeilToInt((float)amount / 100);
        //Debug.Log(amount+", " + numberOfShips + "price of journey:" +t1.name +"->"+ t2.name + (numberOfShips * (t1.transform.position - t2.transform.position).magnitude * 3 + 100));
        return numberOfShips * (t1.transform.position-t2.transform.position).magnitude*5+100;
    }
}
