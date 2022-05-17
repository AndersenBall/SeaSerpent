using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float bombRadius = 5.0F;
    public float bombPower = 10.0F;

    void Start()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, bombRadius);
        foreach (Collider hit in colliders)
        {
           Rigidbody rb = hit.GetComponent<Rigidbody>();

           if (rb != null)
                rb.AddExplosionForce(bombPower, explosionPos, bombRadius, 3.0F);
        }
    }
}
