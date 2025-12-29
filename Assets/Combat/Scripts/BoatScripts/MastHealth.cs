namespace MapMode.Scripts.BoatScripts
{
    using System.Collections;
    using System.Collections.Generic;
    using Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame.Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame;
    using MapMode.Scripts.BoatScripts;
    using UnityEngine;
    using UnityEngine.UI;

    public class MastHealth : ShipHealthComponent
    {
        [SerializeField] private int _currentHealth;
    
        private MastControls mastControls;
        private HUDController hud;

        private Slider healthSlider;
        private Canvas healthCanvas;

        private void Start()
        {
            spawnMinBounds = new Vector3(-3, 0, -3);
            spawnMaxBounds = new Vector3(3, 2, 3);
        
            mastControls = gameObject.GetComponentInParent<MastControls>();

            healthCanvas = GetComponentInChildren<Canvas>();
            if (healthCanvas != null){
                healthSlider = healthCanvas.GetComponentInChildren<Slider>();
                if (healthSlider != null){
                    healthSlider.maxValue = MaxHealth;
                    healthSlider.value = CurrentHealth;
                }
            }


        }
    
        private void Die()
        {
            if (healthCanvas != null){
                healthCanvas.enabled = false;
            }

            mastControls.isDead = true;
        }

        protected override void OnMaxHealthChanged()
        {
            if (healthSlider is not null){
                healthSlider.maxValue = MaxHealth;
            }
        }

        protected override void OnHealthChanged()
        {
            UpdateHUD();
        }

        private void UpdateHUD()
        {
            if (healthSlider is null) return;
            healthSlider.value = CurrentHealth;
            
        }
    
    }

}