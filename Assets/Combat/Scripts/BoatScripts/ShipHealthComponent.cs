using Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame.Combat.Scripts.BoatScripts.BoatAIOld.BoatRepairMiniGame;

namespace MapMode.Scripts.BoatScripts
{
    using UnityEngine;

/// <summary>
/// Generic Health class to be inherited by specific health classes (e.g., BoatHealth).
/// </summary>
public class ShipHealthComponent : MonoBehaviour
{
    [SerializeField]
    protected int _maxHealth = 100;
    
    public virtual int MaxHealth
    {
        get => _maxHealth;
        set
        {
            _maxHealth = Mathf.Clamp(value, 1, 1000);
            OnMaxHealthChanged();
        }
    }

    [SerializeField]
    protected int _currentHealth;

    public virtual int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
            OnHealthChanged();
            if (_currentHealth <= 0 && !IsDead)
            {
                IsDead = true;
                Die();
            }
        }
    }

    public bool IsDead { get; protected set; } = false;

    [SerializeField] private GameObject repairTaskPrefab;
    [SerializeField] private Transform healthSpawnLocation;
    
    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    /// <summary>
    /// Applies damage to this entity.
    /// </summary>
    public virtual void TakeDamage(int damageAmount)
    {
        if (IsDead) return; 

        CurrentHealth -= damageAmount;
        
        if (CurrentHealth <= 0f){
            IsDead = true;
            Die();
        }

        SpawnRepairTask(damageAmount);

    }

    /// <summary>
    /// Applies healing to this entity.
    /// </summary>
    public virtual void Heal(int amount)
    {
        if (IsDead) return;
        CurrentHealth += amount;
    }

    /// <summary>
    /// Called when this entity dies.
    /// Override in child class for custom death logic.
    /// </summary>
    protected virtual void Die()
    {
        // Custom death logic in derived class
    }

    /// <summary>
    /// Called when health changes.
    /// Override to update HUD or effects.
    /// </summary>
    protected virtual void OnHealthChanged()
    {
        // For HUD update in derived class
    }

    /// <summary>
    /// Called when max health changes.
    /// Override to update bars, etc.
    /// </summary>
    protected virtual void OnMaxHealthChanged()
    {
        // For HUD slider max update in derived class
    }
    private void SpawnRepairTask(int healthRestore)
    {
        if (repairTaskPrefab == null || healthSpawnLocation == null){
            Debug.LogWarning("RepairTask prefab or ship transform missing.");
            return;
        }

        // Generate a position for the repair task
        Vector3 randomPosition = healthSpawnLocation.position + new Vector3(
            Random.Range(-5f, 5f), 7f, Random.Range(-5f, 5f));

        // Instantiate the repair task
        GameObject taskInstance = Instantiate(repairTaskPrefab, randomPosition, Quaternion.identity, healthSpawnLocation);
        RepairTask repairTask = taskInstance.GetComponent<RepairTask>();

        if (repairTask != null){
            // Initialize the task with the necessary references and values
            repairTask.Initialize(this, healthRestore);
        }
        
    }
}
}