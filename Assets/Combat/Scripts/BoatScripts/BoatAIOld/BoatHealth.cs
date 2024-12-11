using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatHealth : MonoBehaviour
{

    public float maxHealth = 100f;
    public float currentHealth { get; set; } = 50f;
    private bool isDead = false;
    private BoatControls boatControls;
    private HUDController hud;

    private void Start()
    {
        boatControls = gameObject.GetComponentInParent<BoatControls>();

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

        // Update HUD only if this is the player's boat
        UpdateHUD();
        

        if (currentHealth <= 0f)
        {
            isDead = true;
            Die();
        }
    }

    public void SetHealth(float hp) {
        currentHealth = hp;

    }
   
    public float GetHealth() {
        return currentHealth;
    }
    private void Die() {
        boatControls.Die();
    }

    private void UpdateHUD()
    {
        if (boatControls.GetPlayerControlled() && hud != null)
        {
            hud.UpdateHealth(currentHealth); // Update the HUD with current health
        }
    }

}
