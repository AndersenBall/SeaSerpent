using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CannonAPI : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public Transform barrel;
    public GameObject bulletPrefab;
    public Animator gunAnim;
    public float gunAngle=5;
    private bool isLoaded=true;

    


    public void RotateBarrel(float gunAng)
    {
        barrel.RotateAroundLocal(new Vector3(1f, 0f, 0f), -(gunAng * (2 * Convert.ToSingle(Math.PI)) / 360));
    }

    public void Fire(){
        isLoaded = false;
        gunAnim.SetTrigger("isFiring");
        gunAnim.SetBool("isLoaded", false);
        muzzleFlash.Play();
        GameObject bulletObject = Instantiate(bulletPrefab);
        //assign bullet same tag as the guns root tag.
        bulletObject.layer = transform.root.gameObject.layer;

        //sets direction that object should be facing based off guns directon
        bulletObject.transform.position = barrel.position + barrel.right;
        bulletObject.transform.forward = barrel.right;
    }
    public void LoadGun() {
        StartCoroutine(loadWait());
    }

    public bool GetLoadStatus() {
        return isLoaded;
    }

    IEnumerator loadWait() {
        gunAnim.SetBool("isLoaded", true);
        yield return new WaitForSeconds(2f);
        isLoaded = true;
    }

}
