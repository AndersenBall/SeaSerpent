using System;

namespace MapMode.Scripts
{
    public static class CombatEvents
    {
        public static event Action<Fleet> DefeatFleet;

        public static void InvokeDefeatFleet(Fleet enemyFleet)
        {
            DefeatFleet?.Invoke(enemyFleet);
        }

        public static void Clear()
        {
            DefeatFleet = null;
        }
    }

}