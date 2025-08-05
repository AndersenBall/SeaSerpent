namespace MapMode.Scripts.NavalInteractionContracts
{
    public class AttackFleetEvent
    {
        public Fleet EnemyFleetID { get; private set; } 
        public bool PlayerStartedAttack { get; private set; } // True if the player initiated the attack, false if the enemy started

        // Constructor
        public AttackFleetEvent(Fleet enemyFleetID, bool playerStartedAttack)
        {
            EnemyFleetID = enemyFleetID;
            PlayerStartedAttack = playerStartedAttack;
        }
        
        public override string ToString()
        {
            return $"Attack Event: PlayerInitiated = {PlayerStartedAttack}, " +
                   $"EnemyFleetID = {EnemyFleetID}";
        }
    }

}