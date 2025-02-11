using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Sailor
{
    public string Name { get; private set; }
    public SailorStats SailorStats { get; private set; }

    public Sailor(string n, SailorType type)
    {
        Name = n;

        // Fetch stats from the database
        if (SailorStatsDatabase.BaseStats.TryGetValue(type, out SailorStats stats))
        {
            SailorStats = stats;
        }
        else
        {
            Debug.LogError($"No stats found for SailorType {type}");
            
        }
    }

    public override string ToString()
    {
        return $"name: {Name}" + SailorStats.ToString();
    }
}
