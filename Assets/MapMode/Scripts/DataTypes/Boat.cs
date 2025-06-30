using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Boat
{
    public string boatName;
    public BoatType boatType { get; private set; }
    public BoatStats baseStats { get; private set; }
    public float boatSpeed { get; private set; }
    public float turnSpeed { get; private set; }
    public int boatHealth { get; private set; }

    public int cargoCurrent { get; private set; } = 0;
    public int cargoMax { get; private set; }
    public IDictionary<string, int> supplies = new Dictionary<string,int>();
    public List<Sailor> sailors = new List<Sailor>();
    public int maxSailorCount { get; private set; }

    public Hull hull { get; private set; }
    public Sails sails { get; private set; }
    public Cannon cannon { get; private set; }

    
    public Boat(string name, BoatType type)
    {
        boatName = name;
        boatType = type;

        // Fetch stats from the database
        if (BoatStatsDatabase.BaseStats.TryGetValue(type, out BoatStats boatStats))
        {
            baseStats = boatStats;
            RecalculateStats();
            AddSailor(new Sailor("tom",SailorType.Gunner));
            AddSailor(new Sailor("jerry", SailorType.Gunner));
        }
        else
        {
            Debug.LogError($"No stats found for BoatType {type}");
        }
    }

    public void SetHull(Hull newHull)
    {
        hull = newHull;
        RecalculateStats();
    }

    public void SetSails(Sails newSails)
    {
        sails = newSails;
        RecalculateStats();
    }

    public void SetCannon(Cannon newCannon)
    {
        cannon = newCannon;
        RecalculateStats();
    }

    private void RecalculateStats()
    {
        BoatStats modifiedStats = new BoatStats(
            baseStats.speed,
            baseStats.turnSpeed,
            baseStats.health,
            baseStats.cargoMax,
            baseStats.maxSailorCount,
            baseStats.boatCost
        );

        if (hull != null) hull.ApplyEffect(modifiedStats);
        if (sails != null) sails.ApplyEffect(modifiedStats);
        if (cannon != null) cannon.ApplyEffect(modifiedStats);

        boatSpeed = modifiedStats.speed;
        turnSpeed = modifiedStats.turnSpeed;
        boatHealth = modifiedStats.health;
        cargoMax = modifiedStats.cargoMax;
        maxSailorCount = modifiedStats.maxSailorCount;
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
    public override string ToString()
    {
        return $"Boat Name: {boatName}\n" +
               $"Boat Type: {boatType}\n" +
               $"Base Stats: [{baseStats}]\n" +
               $"Current Stats: [Speed: {boatSpeed}, Turn Speed: {turnSpeed}, Health: {boatHealth}, " +
               $"Cargo Current: {cargoCurrent}/{cargoMax}]\n";
    }


}


