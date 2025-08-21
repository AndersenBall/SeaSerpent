using System;


public static class TownEvents
{
    public static event Action<Town> TownVisited;
    
    public static void InvokeTownVisited(Town town)
    {
        TownVisited?.Invoke(town);
    }
    
    public static void Clear()
    {
        TownVisited = null;
    }
}
