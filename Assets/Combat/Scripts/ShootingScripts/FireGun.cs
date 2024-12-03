using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGun : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public GameObject bulletPrefab;
    public float fireForce = 2000;
    public Transform barrel;
    //public Animator gunAnim;


    // Update is called once per frame
    private void Start()
    {
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        //must update what direction the gun is oriented. This gun is oriented towards up
        if (Input.GetMouseButtonDown(0)) {

            if (muzzleFlash != null) { 
                muzzleFlash.Play();
                Debug.Log("play bullet animation");
            }

            GameObject bulletObject = Instantiate(bulletPrefab, barrel.position+ barrel.forward , Quaternion.identity);
            Rigidbody bulletRigidBody = bulletObject.GetComponent<Rigidbody>();

            //sets direction that object should be facing based off guns directon and fires it
            bulletObject.transform.forward = barrel.forward;
            bulletRigidBody.AddForce(barrel.forward * fireForce);
            
        }
        

    }
}
