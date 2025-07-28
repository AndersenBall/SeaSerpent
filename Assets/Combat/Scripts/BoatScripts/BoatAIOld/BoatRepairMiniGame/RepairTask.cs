using UnityEngine;

namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    using UnityEngine;

namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    public class RepairTask : MonoBehaviour
    {
        private BoatHealth boatHealth;
        private float repairAmount; 
        [SerializeField]
        private RhythmMiniGame rhythmMiniGame; 
        
        public void Initialize(BoatHealth parentHealth, float healthRestore )
        {
            boatHealth = parentHealth;
            repairAmount = healthRestore;
        }

        private void OnMouseDown()
        {
            if (boatHealth != null && rhythmMiniGame != null)
            {
                rhythmMiniGame.onMiniGameCompleted = OnMiniGameCompleted;
                rhythmMiniGame.StartRhythmGame();
            }
        }
         private void OnMiniGameCompleted(bool success)
        {
            if (success)
            {
                if (boatHealth != null)
                {
                    boatHealth.SetHealth(boatHealth.currentHealth + repairAmount);
                    Debug.Log($"Repair successful! Restored {repairAmount} health.");
                }
            }
            else
            {
                Debug.Log("Repair failed!");
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