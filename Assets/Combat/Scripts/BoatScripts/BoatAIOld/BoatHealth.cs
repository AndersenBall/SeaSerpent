using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatHealth : MonoBehaviour
{
    public float health = 50f;
    private bool dead = false;
    private BoatControls boatControls;

    private HUDController hud;

    private void Start()
    {
        boatControls = gameObject.GetComponentInParent<BoatControls>();
        hud = GameObject.Find("HUD/Canvas").GetComponent<HUDController>();

    }


    public void TakeDamage(float dmgAmount) {
        //Debug.Log(gameObject.transform.parent.name + " taken dmg: " + dmgAmount);
        
        health -= dmgAmount;
        if (health <= 0f && dead !=true) {
            dead = true;
            Die();
        }
        if (boatControls.GetPlayerControlled()) {
            hud.UpdateHealth(health);
        }
    }
    public void SetHealth(float hp,bool playerBoat) {
        health = hp;
        if (playerBoat) {
            hud = GameObject.Find("HUD/Canvas").GetComponent<HUDController>();//start method is called too late
            hud.UpdateHealth(hp);
        }
    }
   
    public float GetHealth() {
        return health;
    }
    private void Die() {
        boatControls.Die();
    }
}
