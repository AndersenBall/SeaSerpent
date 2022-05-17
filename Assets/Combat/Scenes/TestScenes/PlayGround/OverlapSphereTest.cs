using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapSphereTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ExplosionDamage(transform.position, 10.0f);
    }

    void ExplosionDamage(Vector3 center, float radius)
    {
        
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders) {
            Debug.Log(hitCollider.gameObject);
        }
        
    }
}
