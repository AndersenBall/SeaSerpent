using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this script is to launch a bomb out of a gun and then have it explode on impact.
public class BombStart : MonoBehaviour
{
    // Start is called before the first frame update
    public float bombRadius = 5.0F;
    public float bombPower = 10.0F;
    public float damageDeal = 10;
    public GameObject impactEffect1;
    public GameObject impactEffect2;

    public Vector3 acceleration;
    public Vector3 lastVelocity = new Vector3(0,0,0);

    private bool hasCollided = false;
    private void Start() {
        Destroy(this.gameObject, 30);
    }
    private void Update()
    {
        acceleration = (this.GetComponent<Rigidbody>().velocity - lastVelocity) / Time.fixedDeltaTime;
        lastVelocity = this.GetComponent<Rigidbody>().velocity;
        //Debug.Log("Velocity:" + this.GetComponent<Rigidbody>().velocity.z + " " + this.GetComponent<Rigidbody>().velocity.y);
    }
    //this sets off a bomb explossion on collison. Setting off physics explosion and animation on collision.
    private void OnCollisionEnter(Collision collision)
    {
        
        if (hasCollided == false) {
            hasCollided = true;
            Debug.Log("bomb hit: "+ collision.transform.name);

            if ( null != collision.gameObject.GetComponent<BoatHealth>()) {
                BoatHealth hitBoatHealth = collision.gameObject.GetComponent<BoatHealth>();
                hitBoatHealth.TakeDamage(damageDeal);
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
