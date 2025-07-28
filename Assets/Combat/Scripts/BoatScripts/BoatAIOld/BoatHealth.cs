using System.Collections;
using System.Collections.Generic;
using Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame.Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame;
using UnityEngine;
using UnityEngine.UI;

public class BoatHealth : MonoBehaviour
{

    public float maxHealth = 100f;

    [SerializeField] private float _currentHealth;

    public float currentHealth{
        get{ return _currentHealth; }
        set{ _currentHealth = value; }
    }

    private bool isDead = false;
    private BoatControls boatControls;
    private HUDController hud;

    private Slider healthSlider;
    private Canvas healthCanvas;

    [SerializeField] private GameObject repairTaskPrefab;
    [SerializeField] private Transform shipTransform;


    private void Start()
    {
        currentHealth = maxHealth;
        boatControls = gameObject.GetComponentInParent<BoatControls>();

        healthCanvas = GetComponentInChildren<Canvas>();
        if (healthCanvas != null){
            healthSlider = healthCanvas.GetComponentInChildren<Slider>();
            if (healthSlider != null){
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

        UpdateHUD();


        if (currentHealth <= 0f){
            isDead = true;
            Die();
        }

        SpawnRepairTask(damageAmount);

    }

    public void SetHealth(float hp)
    {
        currentHealth = Mathf.Clamp(hp, 0, maxHealth);

        // Update the health bar
        if (healthSlider != null){
            healthSlider.value = currentHealth;
        }

    }

    public float GetHealth()
    {
        return currentHealth;
    }

    private void Die()
    {
        if (healthCanvas != null){
            healthCanvas.enabled = false;
        }

        boatControls.Die();
    }

    private void UpdateHUD()
    {
        healthSlider.value = currentHealth;

        if (boatControls.GetPlayerControlled() && hud != null){
            hud.UpdateHealth(currentHealth); // Update the HUD with current health
        }
    }

    private void SpawnRepairTask(float healthRestore)
    {
        if (repairTaskPrefab == null || shipTransform == null){
            Debug.LogWarning("RepairTask prefab or ship transform missing.");
            return;
        }

        // Generate a position for the repair task
        Vector3 randomPosition = shipTransform.position + new Vector3(
            Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));

        // Instantiate the repair task
        GameObject taskInstance = Instantiate(repairTaskPrefab, randomPosition, Quaternion.identity, shipTransform);
        RepairTask repairTask = taskInstance.GetComponent<RepairTask>();

        if (repairTask != null){
            // Initialize the task with the necessary references and values
            repairTask.Initialize(this, healthRestore);
        }



    }
}
