using MapMode.Scripts.BoatScripts;
using UnityEngine;

namespace Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame
{
    public class RepairTask : MonoBehaviour
    {
        private ShipHealthComponent boatHealth;
        private int maxRepairAmount;

        [SerializeField] private GameObject rhythmMiniGamePrefab;
        [SerializeField] private Transform uiParent;

        private RhythmMiniGame activeMiniGame;
        private bool miniGameRunning;

        public void Initialize(ShipHealthComponent parentHealth, int healthRestore)
        {
            boatHealth = parentHealth;
            maxRepairAmount = Mathf.Max(0, healthRestore);
        }

        public void startMiniGame()
        {
            if (miniGameRunning || boatHealth == null)
            {
                return;
            }

            if (rhythmMiniGamePrefab == null)
            {
                Debug.LogWarning("RepairTask is missing a rhythmMiniGamePrefab reference.");
                return;
            }

            miniGameRunning = true;
            Transform parent = uiParent;

            GameObject miniGameInstance = parent == null
                ? Instantiate(rhythmMiniGamePrefab)
                : Instantiate(rhythmMiniGamePrefab, parent);

            activeMiniGame = miniGameInstance.GetComponent<RhythmMiniGame>();
            if (activeMiniGame == null)
            {
                Debug.LogWarning("Mini-game prefab does not contain a RhythmMiniGame component.");
                miniGameRunning = false;
                Destroy(miniGameInstance);
                return;
            }

            activeMiniGame.onMiniGameCompleted += OnMiniGameCompleted;
            activeMiniGame.StartRhythmGame();
        }

        private void OnMiniGameCompleted(float accuracy)
        {
            if (activeMiniGame != null)
            {
                activeMiniGame.onMiniGameCompleted -= OnMiniGameCompleted;
            }

            if (boatHealth != null)
            {
                int repairAmount = Mathf.RoundToInt(maxRepairAmount * Mathf.Clamp01(accuracy));
                boatHealth.Heal(repairAmount);
                Debug.Log($"Repair successful! Restored {repairAmount} out of {maxRepairAmount} health.");
            }

            if (activeMiniGame != null)
            {
                Destroy(activeMiniGame.gameObject);
            }

            RemoveTask();
        }

        public void RemoveTask()
        {
            Destroy(gameObject);
        }
    }
}
