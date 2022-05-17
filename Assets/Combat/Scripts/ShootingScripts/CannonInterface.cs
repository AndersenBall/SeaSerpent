using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class CannonInterface : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public Transform barrel;
    public Transform rotationPoint;
    public GameObject bulletPrefab;

    private Animator gunAnim;
    private AudioSource cannonAudioSource;
    private HaloController haloControl;
    private CannonLine cannonLine;
    
    public float gunAngle=0;
    public float wantedGunAngle = 0;
    public float fireForce = 100;
    private float cannonVariance = 1;
    private bool isLoaded = true;
    private bool isBeingWorkedOn = false;
    public int cannonSetNum;

    private void Start()
    {
        cannonLine = gameObject.GetComponentInChildren<CannonLine>();
        gunAnim = gameObject.GetComponent<Animator>();
        cannonAudioSource = gameObject.GetComponentInChildren<AudioSource>();
        haloControl = gameObject.GetComponentInChildren<HaloController>();
    }

    public bool GetNeedsRotation() {
        if (wantedGunAngle == gunAngle)
            return false;
        else
            return true;
    }
    public void RotateBarrel() // amount to rotate the gun
    {
        cannonLine.RotationOffset(0);
        float rotationAmount = wantedGunAngle - gunAngle;
        gunAngle = wantedGunAngle;
        rotationPoint.Rotate(rotationAmount, 0, 0);
    }
    public void UpdateWantedBarrelAngle(float ang) {//set amount you want the cannon to be at, for computer.
        
        if (wantedGunAngle - ang > -25 && wantedGunAngle - ang < 10) {
            wantedGunAngle -= ang;
            cannonLine.RotationOffset(ang);
        }
    }
    public void SetWantedBarrelAngle(float ang)
    {//set amount you want the cannon to be at
        
        float prevWanted = wantedGunAngle;
        wantedGunAngle = -Mathf.Clamp(ang, -10, 25); ;
        cannonLine.RotationOffset(prevWanted-wantedGunAngle);
    }

    public void SetLineActivity(bool activity) {
        cannonLine.SetActive(activity);
    }

    public void Fire(){
        if (isLoaded == true) {
            isLoaded = false;
            isBeingWorkedOn = false;
            gunAnim.SetTrigger("isFiring");
            gunAnim.SetBool("isLoaded", false);
            muzzleFlash.Play();
            cannonAudioSource.Play();
            haloControl.SetHalo(true);

            //spawns and pushes bullet
            GameObject bulletObject = Instantiate(bulletPrefab, barrel.position , Quaternion.identity);
            Rigidbody bulletRigidBody = bulletObject.GetComponent<Rigidbody>();

            //sets direction that object should be facing based off guns directon and fires it
            bulletObject.transform.forward = barrel.forward;
            //bulletRigidBody.AddForce(transform.forward * fireForce);
            UnityEngine.Random rand = new UnityEngine.Random(); //reuse this if you are generating many
            double u1 = 1.0 - UnityEngine.Random.Range(0,.99f); //uniform(0,1] random doubles
            double u2 = 1.0 - UnityEngine.Random.Range(0f,.99f);
            //Debug.Log("random number:" + u1 + " " + u2);
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            float randNormalX =   cannonVariance * (float)randStdNormal;
            u1 = 1.0 - UnityEngine.Random.Range(0, .99f); //uniform(0,1] random doubles
            u2 = 1.0 - UnityEngine.Random.Range(0f, .99f);
            //Debug.Log("random number:" + u1 + " " + u2);
            randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            float randNormalY = cannonVariance * (float)randStdNormal;

            
            //Debug.Log(randNormalX + "" + randNormalY);
            bulletRigidBody.velocity = Quaternion.AngleAxis(randNormalX, Vector3.up)* Quaternion.AngleAxis(randNormalY,Vector3.right) * barrel.forward * fireForce;
        }
        
    }
    public float GetFireSpeed() {
        return fireForce;
    }
    public void setCannonSetNum(int set) {
        cannonSetNum = set;
    }

    public int getCannonSetNum()
    {
        return cannonSetNum;
    }

    public bool GetLoadStatus() {
        return isLoaded;
    }
    public void SetBusyStatus(bool busyStatus) {
        
        isBeingWorkedOn = busyStatus;
    }
    public bool GetBusyStatus()
    {
        return isBeingWorkedOn;
    }


    //load the cannon
    public void LoadGun()
    {
        if (isLoaded == false) {
            StartCoroutine(LoadWait());
        }
    }
    IEnumerator LoadWait() {
        gunAnim.SetBool("isLoaded", true);
        yield return new WaitForSeconds(2f);
        haloControl.SetHalo(false);
        isLoaded = true;
        isBeingWorkedOn = false;
    }

}
