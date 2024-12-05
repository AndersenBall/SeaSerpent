using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//not full sure what this code does
public class BulletStart
    : MonoBehaviour
{
    // Start is called before the first frame update
    public float bombRadius = 5.0F;
    public float bombPower = 10.0F;
    public float damageDeal = 10;
    public GameObject impactEffect1;
    public GameObject impactEffect2;



    private bool hasCollided = false;
    private void Start()
    {
        Destroy(this.gameObject, 20);
    }

    //this sets off a bomb explossion on collison. Setting off physics explosion and animation on collision.
    private void OnCollisionEnter(Collision collision)
    {

        if (hasCollided == false) {
            hasCollided = true;
            Debug.Log("bomb hit: " + collision.transform.name);

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
        makeExplosionForce(transform.position);

    }
    private void makeExplosionForce(Vector3 explosionPos)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPos, bombRadius);
        foreach (Collider hit in colliders) {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null) {
                rb.AddExplosionForce(bombPower, explosionPos, bombRadius, 3.0F);
            }
        }


    }
}
