using System;

namespace MapMode.Scripts
{
    public class CombatEvents
    {
        public static event Action<string> EnemyKilled;

        public static void InvokeEnemyKilled(string enemyID)
        {
            EnemyKilled?.Invoke(enemyID);
        }

        public static void Clear()
        {
            EnemyKilled = null;
        }
    }

}