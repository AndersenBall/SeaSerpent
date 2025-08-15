namespace MapMode.Scripts.NavalInteractionContracts
{
    public class AttackFleetEvent
    {
        public Fleet EnemyFleet { get; private set; } 
        public bool PlayerStartedAttack { get; private set; } // True if the player initiated the attack, false if the enemy started
        
        public AttackFleetEvent(Fleet enemyFleet, bool playerStartedAttack)
        {
            EnemyFleet = enemyFleet;
            PlayerStartedAttack = playerStartedAttack;
        }
        
        public override string ToString()
        {
            return $"Attack Event: PlayerInitiated = {PlayerStartedAttack}, " +
                   $"EnemyFleetID = {EnemyFleet}";
        }
    }

}