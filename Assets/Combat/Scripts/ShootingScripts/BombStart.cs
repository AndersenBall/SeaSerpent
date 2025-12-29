using System.Collections;
using System.Collections.Generic;
using MapMode.Scripts.BoatScripts;
using UnityEngine;


//this script is to launch a bomb out of a gun and then have it explode on impact.
public class BombStart : MonoBehaviour
{
    // Start is called before the first frame update
    public float bombRadius = 5.0F;
    public float bombPower = 10.0F;
    public int damageDeal = 5;
    public GameObject impactEffect1;
    public GameObject impactEffect2;

 
    private bool hasCollided = false;
    private void Start() {
        Destroy(this.gameObject, 30);
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        
        if (!hasCollided ) {
            hasCollided = true;
            Debug.Log("bomb hit: "+ collision.transform.name);
            Vector3 hitPoint = collision.contacts[0].point;
            
            if ( null != collision.gameObject.GetComponent<ShipHealthComponent>()) {
                ShipHealthComponent hitBoatHealth = collision.gameObject.GetComponent<ShipHealthComponent>();
                hitBoatHealth.TakeDamage(damageDeal,hitPoint);
            }

            if (null != collision.gameObject.GetComponent<Health>()) {
                Health health = collision.gameObject.GetComponent<Health>();
                health.TakeDamage(damageDeal);
            }

            GameObject impactAnimation1 = Instantiate(impactEffect1, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));
            GameObject impactAnimation2 = Instantiate(impactEffect2, transform.position, Quaternion.LookRotation(collision.contacts[0].normal));

            Vector3 explosionPos = transform.position;
            

            Destroy(gameObject, .5f);
            Destroy(impactAnimation1, 2);
            Destroy(impactAnimation2, 2);
        }

    }
    private void makeExplosionForce(Vector3 explosionPos) {
        Collider[] colliders = Physics.OverlapSphere(explosionPos, bombRadius);
        foreach (Collider hit in colliders) {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null) {
                rb.AddExplosionForce(bombPower, explosionPos, bombRadius, 3.0F);
            }
        }


    }
}
