using System;
using UnityEngine;

public class TownUIContext : MonoBehaviour
{
    public Town CurrentTown { get; private set; }
    public Fleet CurrentFleet { get; private set; }

    public event Action<Town, Fleet> ContextChanged;

    public void SetContext(Town town, Fleet fleet)
    {
        CurrentTown = town;
        CurrentFleet = fleet;
        ContextChanged?.Invoke(CurrentTown, CurrentFleet);
    }
}
