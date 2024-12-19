using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float health = 50f;

    public void TakeDamage(float dmgAmount) {
        Debug.Log("taken dmg: " + dmgAmount);
        health -= dmgAmount;
        if (health <= 0f) {
            Die();
        }
    }

    private void Die() {
        PandaUnitAI pandaUnitAI = GetComponent<PandaUnitAI>();
        if (pandaUnitAI != null)
        {
            // Call the Die function in PandaUnitAI
            pandaUnitAI.Die();
        }
        
        Debug.LogWarning("unit died:" + gameObject.name);
        Destroy(gameObject);
        
    }
}
