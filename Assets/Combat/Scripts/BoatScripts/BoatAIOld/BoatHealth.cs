using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatHealth : MonoBehaviour
{
    public float health = 50f;
    private bool dead = false;
    private BoatControls boatControls;

    private void Start()
    {
        boatControls = gameObject.GetComponentInParent<BoatControls>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("]")) {
            TakeDamage(0);
        }
    }
    public void TakeDamage(float dmgAmount) {
        //Debug.Log(gameObject.transform.parent.name + " taken dmg: " + dmgAmount);
        
        health -= dmgAmount;
        if (health <= 0f && dead !=true) {
            dead = true;
            Die();
        }
    }
    public void SetHealth(float hp) {
        health = hp;
    }
    public float GetHealth() {
        return health;
    }
    private void Die() {
        boatControls.Die();
    }
}
