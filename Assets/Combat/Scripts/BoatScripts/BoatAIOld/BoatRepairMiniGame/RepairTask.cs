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
        private GameObject rhythmMiniGamePrefab;

        private Transform uiParent; 

        
        private RhythmMiniGame rhythmMiniGame; 
        
        public void Initialize(BoatHealth parentHealth, int healthRestore )
        {
            boatHealth = parentHealth;
            maxRepairAmount = healthRestore;
        }

        //TODO wont work. need to use a raycast
        public void startMiniGame()
        {
            Debug.Log("starting mini game");
            if (boatHealth == null) return;

            GameObject miniGameInstance = Instantiate(rhythmMiniGamePrefab, uiParent);

            RhythmMiniGame miniGameScript = miniGameInstance.GetComponent<RhythmMiniGame>();
            if (miniGameScript != null)
            {
                miniGameScript.onMiniGameCompleted = OnMiniGameCompleted;
                miniGameScript.StartRhythmGame();
            }

        }
         private void OnMiniGameCompleted(float accuracy)
        {
           
            if (boatHealth != null)
            {
                int repairAmount = Mathf.RoundToInt(maxRepairAmount * accuracy);
                boatHealth.currentHealth += repairAmount;
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