using System.Collections;
using System.Collections.Generic;
using Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame.Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame;
using UnityEngine;
using UnityEngine.UI;

public class BoatHealth : MonoBehaviour
{
    [SerializeField]
    public int _maxHealth = 100;
    public int maxHealth{
        get{ return _maxHealth; }
        set{
            _maxHealth = Mathf.Clamp(value, 1, 1000); 
            if (healthSlider is not null){
                healthSlider.maxValue = maxHealth;
            }
        }
    }
    
    [SerializeField] private int _currentHealth;

    public int currentHealth{
        get{ return _currentHealth; }
        set{ 
            _currentHealth = Mathf.Clamp(value, 0, maxHealth);

            UpdateHUD();
        }
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
        
        boatControls = gameObject.GetComponentInParent<BoatControls>();

        healthCanvas = GetComponentInChildren<Canvas>();
        if (healthCanvas != null){
            healthSlider = healthCanvas.GetComponentInChildren<Slider>();
            if (healthSlider != null){
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }
        
        if (boatControls.GetPlayerControlled()) 
        {
            hud = GameObject.Find("HUD/Canvas")?.GetComponent<HUDController>();
            UpdateHUD();
        }

    }
    
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; 

        currentHealth -= damageAmount;
        
        if (currentHealth <= 0f){
            isDead = true;
            Die();
        }

        SpawnRepairTask(damageAmount);

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
        if (healthSlider is null) return;
        healthSlider.value = currentHealth;

        if (boatControls.GetPlayerControlled() && hud is not null){
            hud.UpdateHealth(currentHealth); 
        }
    }

    private void SpawnRepairTask(int healthRestore)
    {
        if (repairTaskPrefab == null || shipTransform == null){
            Debug.LogWarning("RepairTask prefab or ship transform missing.");
            return;
        }

        // Generate a position for the repair task
        Vector3 randomPosition = shipTransform.position + new Vector3(
            Random.Range(-5f, 5f), 7f, Random.Range(-5f, 5f));

        // Instantiate the repair task
        GameObject taskInstance = Instantiate(repairTaskPrefab, randomPosition, Quaternion.identity, shipTransform);
        RepairTask repairTask = taskInstance.GetComponent<RepairTask>();

        if (repairTask != null){
            // Initialize the task with the necessary references and values
            repairTask.Initialize(this, healthRestore);
        }



    }
}
