using MapMode.Scripts.DataTypes.boatComponents.Cannons;
using UnityEngine;

public class CannonStationSpawner : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private GameObject cannonPrefab;
    [SerializeField] private Transform cannonSpawnPoint;
    [SerializeField] private Transform spawnedCannonParent;
    [Header("Control Trigger")]
    [SerializeField] private Collider controlTrigger;
    [SerializeField] private int cannonSetNumber;

    private CannonInterface spawnedCannon;

    public CannonInterface SpawnedCannon => spawnedCannon;

    private void Start()
    {
        EnsureTriggerIsConfigured();
    }

    public CannonInterface SpawnCannon(Cannon cannonConfig)
    {
        if (cannonConfig == null)
        {
            Debug.LogError($"{name}: Cannon config is null. Boat must provide a cannon configuration during spawn.");
            return null;
        }

        if (cannonPrefab == null)
        {
            Debug.LogWarning($"{name}: Cannon prefab is not set on {nameof(CannonStationSpawner)}.");
            return null;
        }

        if (spawnedCannon != null)
        {
            Destroy(spawnedCannon.gameObject);
            spawnedCannon = null;
        }

        var spawnPoint = cannonSpawnPoint != null ? cannonSpawnPoint : transform;
        var parent = spawnedCannonParent != null ? spawnedCannonParent : transform;

        GameObject spawnedObject = Instantiate(cannonPrefab, spawnPoint.position, spawnPoint.rotation, parent);
        spawnedObject.name = $"{cannonPrefab.name}_Spawned";

        spawnedCannon = spawnedObject.GetComponent<CannonInterface>();
        if (spawnedCannon == null)
        {
            Debug.LogError($"{name}: Spawned cannon prefab does not contain a {nameof(CannonInterface)} component.");
            return null;
        }

        spawnedCannon.setCannonSetNum(cannonSetNumber);
        spawnedCannon.SetCannonValues(cannonConfig);

        return spawnedCannon;
    }

    private void EnsureTriggerIsConfigured()
    {
        if (controlTrigger == null)
        {
            controlTrigger = GetComponent<Collider>();
        }

        if (controlTrigger == null)
        {
            return;
        }

        controlTrigger.isTrigger = true;
        if (!controlTrigger.CompareTag("Cannon"))
        {
            controlTrigger.tag = "Cannon";
        }
    }
}
