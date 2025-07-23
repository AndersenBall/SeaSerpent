using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatHealth : MonoBehaviour
{

    public float maxHealth = 100f;
    
    [SerializeField] private float _currentHealth;
    public float currentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }
    private bool isDead = false;
    private BoatControls boatControls;
    private HUDController hud;

    private Slider healthSlider; 
    private Canvas healthCanvas; 

    private void Start()
    {
        currentHealth = maxHealth;
        boatControls = gameObject.GetComponentInParent<BoatControls>();

        healthCanvas = GetComponentInChildren<Canvas>();
        if (healthCanvas != null)
        {
            healthSlider = healthCanvas.GetComponentInChildren<Slider>();
            if (healthSlider != null)
            {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }

        if (boatControls.GetPlayerControlled()) // Only find HUD if this is the player's boat
        {
            hud = GameObject.Find("HUD/Canvas")?.GetComponent<HUDController>();
            UpdateHUD(); 
        }

    }


    public void TakeDamage(float damageAmount)
    {
        if (isDead) return; // Ignore damage if already dead

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0
        healthSlider.value = currentHealth;

        // Update HUD only if this is the player's boat
        UpdateHUD();
        

        if (currentHealth <= 0f)
        {
            isDead = true;
            Die();
        }
    }

    public void SetHealth(float hp)
    {
        currentHealth = Mathf.Clamp(hp, 0, maxHealth);

        // Update the health bar
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

    }

    public float GetHealth() {
        return currentHealth;
    }
    private void Die() {
        if (healthCanvas != null)
        {
            healthCanvas.enabled = false; 
        }
        boatControls.Die();
    }

    private void UpdateHUD()
    {
        healthSlider.value = currentHealth;

        if (boatControls.GetPlayerControlled() && hud != null)
        {
            hud.UpdateHealth(currentHealth); // Update the HUD with current health
        }
    }

}
