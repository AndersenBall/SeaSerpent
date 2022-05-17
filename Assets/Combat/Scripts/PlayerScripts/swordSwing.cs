using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordSwing : MonoBehaviour
{
    public Animator anim;
    public float damage = 10f;

    private Collider swordCollider;
    private void Start()
    {
        swordCollider = GetComponent<Collider>();
        Debug.Log(swordCollider.name);
        
    }

    void Update()
    {
        //to start sword swing animation
        if (Input.GetMouseButtonDown(0)) {
            anim.SetTrigger("isAttackDown");
        }
        if (Input.GetMouseButtonDown(1)) {

        }
   
    }

    //when sword hits a object check to deal dmg
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
 
        //checks if you are attacking and if there is a valid health to decrease on target. If yes, take damage
        if (collision.gameObject.GetComponent<Health>() != null && IsAttacking())
        {
            collision.gameObject.GetComponent<Health>().TakeDamage(damage);
        }
    }

    //checks to see if a current attack animation is running or not
    private bool IsAttacking() {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SwordSwing"))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
}

