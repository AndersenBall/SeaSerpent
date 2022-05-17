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
        Destroy(gameObject);
    }
}
