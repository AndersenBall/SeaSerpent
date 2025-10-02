using System.Collections;
using System.Collections.Generic;
using Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame.Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame;
using MapMode.Scripts.BoatScripts;
using UnityEngine;
using UnityEngine.UI;

public class BoatHealth : ShipHealthComponent
{
    [SerializeField] private int _currentHealth;
    
    private BoatControls boatControls;
    private HUDController hud;

    private Slider healthSlider;
    private Canvas healthCanvas;

    private void Start()
    {
        spawnMinBounds = new Vector3(-10, 0, -20);
        spawnMaxBounds = new Vector3(10, 5, 20);
        
        boatControls = gameObject.GetComponentInParent<BoatControls>();

        healthCanvas = GetComponentInChildren<Canvas>();
        if (healthCanvas != null){
            healthSlider = healthCanvas.GetComponentInChildren<Slider>();
            if (healthSlider != null){
                healthSlider.maxValue = MaxHealth;
                healthSlider.value = CurrentHealth;
            }
        }
        
        if (boatControls.GetPlayerControlled()) 
        {
            hud = GameObject.Find("HUD/Canvas")?.GetComponent<HUDController>();
            UpdateHUD();
        }

    }
    
    private void Die()
    {
        if (healthCanvas != null){
            healthCanvas.enabled = false;
        }

        boatControls.Die();
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

        if (boatControls.GetPlayerControlled()){
            hud.UpdateHealth(CurrentHealth); 
        }
    }
    
}
