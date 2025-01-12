using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Boat
{
    public string boatName;
    BoatType boatType { get; set; }
    float boatSpeed;
    float turnSpeed;
    int boatHealth;
    IDictionary<string, int> supplies = new Dictionary<string,int>();
    
    int cargoCurrent =0;
    int cargoMax;
    List<Sailor> sailors = new List<Sailor>();
    int maxSailorCount;
   
    
    public Boat(string name, BoatType type ) {
        boatName = name;
        boatType = type;
        if (type == BoatType.ManOfWar) {
            boatSpeed = 5f;
            turnSpeed = .2f;
            boatHealth = 900;
            cargoMax = 1000;
            maxSailorCount = 70;
        }
        else if (type == BoatType.Frigate) {
            boatSpeed = 8f;
            turnSpeed = .3f;
            boatHealth = 100;
            cargoMax = 50;
            maxSailorCount = 10;
            for (int i = 0; i < 6; i++) {
                AddSailor(new Sailor("tom"+ name +i));
            }
        }
        else if (type == BoatType.TradeShip) {
            boatSpeed = 4;
            turnSpeed = .2f;
            boatHealth = 50;
            cargoMax = 100;
            maxSailorCount = 6;
            AddSailor(new Sailor("tom"));
            AddSailor(new Sailor("jerry"));
        }
       
    }
    public int AddCargo(string item, int amount)
    {
        if (amount + cargoCurrent <= cargoMax) {
            cargoCurrent += amount;
            if (supplies.ContainsKey(item)) {
                supplies[item] += amount;
            }
            else {
                supplies.Add(item, amount);
            }
            
            return amount;
            //Debug.Log(boatName+"loaded :" + item + " amount: " + amount);
        }
        else {
            //Debug.Log(boatName + "can not hold " + amount + "of " + item + "added: " + (cargoMax - cargoCurrent));
            cargoCurrent += cargoMax - cargoCurrent;
            supplies.Add(item, cargoMax - cargoCurrent);
            if (supplies.ContainsKey(item)) {
                supplies[item] += cargoMax - cargoCurrent;
            }
            else {
                supplies.Add(item, cargoMax - cargoCurrent);
            }
            return cargoMax - cargoCurrent;
        }
        
    }
    public int RemoveCargo(string item, int amount) {
        if (supplies.ContainsKey(item)) { 
            if (supplies[item] - amount >= 0) {
                cargoCurrent -= amount;
                supplies[item] -= amount; 
                //Debug.Log(boatName + "removed :" + item + " amount: " + amount);
                return amount;
            }
            else {
                Debug.Log(boatName + item+ " can not remove: " + amount + " removed only: " + supplies[item]);
                amount = supplies[item];
                
                supplies[item] = 0;
                return amount;
            }
        }
        Debug.Log("invalid item to remove: " + item);
        return 0;
    }
    public IDictionary<string, int> getSupplies() {
        return supplies;
        
    }
    public float GetSpeed() {return boatSpeed; }
    public float GetTurnSpeed() { return turnSpeed; }
    public int GetCargoMax(){ return cargoMax; }
    public int GetCargoCurrent() { return cargoCurrent; }
    public void AddSailor(Sailor s) {
        sailors.Add(s);
    }
    public List<Sailor> GetSailors() {
        return sailors;
    }
    public int GetBoatHealth() {
        return boatHealth;
    }

    public void SetBoatHealth(int hp) {
        boatHealth = hp;
    }

}


