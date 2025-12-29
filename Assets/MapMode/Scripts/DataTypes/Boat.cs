using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MapMode.Scripts.DataTypes.boatComponents.Cannons;
using MapMode.Scripts.DataTypes.boatComponents.Hulls;
using MapMode.Scripts.DataTypes.boatComponents.Sails;
using UnityEngine;


[System.Serializable]
public class Boat
{
    #region Properties

    public string BoatId;
    
    public string boatName;
    public BoatType boatType { get; private set; }
    public BoatStats baseStats { get; private set; }
    public float boatSpeed { get; private set; }
    public float turnSpeed { get; private set; }
    public int maxBoatHealth { get; private set; }
    
    public int currentBoatHealth { get; set; }

    public int cargoCurrent { get; private set; } = 0;
    public int cargoMax { get; private set; }
    public IDictionary<string, int> supplies = new Dictionary<string,int>();
    public List<Sailor> sailors = new List<Sailor>();
    public int maxSailorCount { get; private set; }

    public Hull hull { get; private set; }
    public Sails sails { get; private set; }
    public Cannon cannon { get; private set; }
    #endregion

    #region Constructor
    public Boat(string name, BoatType type)
    {
        boatName = name;
        boatType = type;

        // Fetch stats from the database
        if (BoatStatsDatabase.BaseStats.TryGetValue(type, out BoatStats boatStats))
        {
            baseStats = boatStats;
            currentBoatHealth = baseStats.maxHealth;
            RecalculateStats();
            AddSailor(new Sailor("tom",SailorType.Gunner));
            AddSailor(new Sailor("jerry", SailorType.Gunner));
        }
        else
        {
            Debug.LogError($"No stats found for BoatType {type}");
        }
    }
    #endregion

    #region Component Management
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
            baseStats.maxHealth,
            baseStats.cargoMax,
            baseStats.maxSailorCount,
            baseStats.boatCost
        );

        if (hull != null) hull.ApplyEffect(modifiedStats);
        if (sails != null) sails.ApplyEffect(modifiedStats);
        if (cannon != null) cannon.ApplyEffect(modifiedStats);

        boatSpeed = modifiedStats.speed;
        turnSpeed = modifiedStats.turnSpeed;
        maxBoatHealth = modifiedStats.maxHealth;
        currentBoatHealth = Mathf.Max(1,modifiedStats.maxHealth - maxBoatHealth + currentBoatHealth);
        cargoMax = modifiedStats.cargoMax;
        maxSailorCount = modifiedStats.maxSailorCount;
    }
    #endregion

    #region Cargo Management 
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
        }
        else {
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
    #endregion

    #region Getters
    
    public float GetMaxCannonRange()
    {
        var c = cannon ?? new Cannon(CannonType.LongGun);              // cache a default instance somewhere
        float v = c.ShotPower;                         
        float g = Mathf.Abs(Physics.gravity.y);        
        float theta = Mathf.Abs(c.MaxVerticalAngle * Mathf.Deg2Rad); 
        
        float R = (v * v * Mathf.Sin(2f * theta)) / g;
        return R;
    }
    

    public IDictionary<string, int> getSupplies() {
        return supplies;
    }
    public float GetSpeed() {return boatSpeed; }
    public float GetTurnSpeed() { return turnSpeed; }
    public int GetCargoMax(){ return cargoMax; }
    public int GetCargoCurrent() { return cargoCurrent; }
    
    
    #endregion

    #region Crew Management
    public void AddSailor(Sailor s) {
        sailors.Add(s);
    }

    public bool RemoveSailor(Sailor sailor) {
        if (sailors.Contains(sailor)) {
            sailors.Remove(sailor);
            return true;
        }
        return false;
    }
    
    public List<Sailor> GetSailors() {
        return sailors;
    }
    #endregion

    #region Health Management
    public int GetBoatHealth() {
        return currentBoatHealth;
    }

    public void SetBoatHealth(int hp) {
        maxBoatHealth = hp;
    }
    #endregion

    #region Overrides
    public override string ToString()
    {
        return $"Boat Name: {boatName}\n" +
               $"Boat Type: {boatType}\n" +
               $"Base Stats: [{baseStats}]\n" +
               $"Current Stats: [Speed: {boatSpeed}, Turn Speed: {turnSpeed}, Health: {maxBoatHealth}, " +
               $"Cargo Current: {cargoCurrent}/{cargoMax}]\n";
    }
    #endregion
}