using UnityEngine;

namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    using UnityEngine;

namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    public class RepairTask : MonoBehaviour
    {
        private BoatHealth boatHealth;
        private int maxRepairAmount; 
        [SerializeField]
        private RhythmMiniGame rhythmMiniGame; 
        
        public void Initialize(BoatHealth parentHealth, int healthRestore )
        {
            boatHealth = parentHealth;
            maxRepairAmount = healthRestore;
        }

        private void OnMouseDown()
        {
            if (boatHealth != null && rhythmMiniGame != null)
            {
                rhythmMiniGame.onMiniGameCompleted = OnMiniGameCompleted;
                rhythmMiniGame.StartRhythmGame();
            }
        }
         private void OnMiniGameCompleted(float accuracy)
        {
           
            if (boatHealth != null)
            {
                int repairAmount = Mathf.RoundToInt(maxRepairAmount * accuracy);
                boatHealth.currentHealth = boatHealth.currentHealth + repairAmount;
                Debug.Log($"Repair successful! Restored {repairAmount } our of {maxRepairAmount} health.");
            }
            
            
            RemoveTask();
        }
         
        public void RemoveTask()
        {
            Destroy(gameObject);
        }
    }
}

}