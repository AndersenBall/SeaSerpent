using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using System.Drawing;

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
    
    private float gunAngle=0;
    private float wantedGunAngle = 0;
    private float horizontalGunAngle = 0; 
    private float cannonVariance = 1.2f;
    private bool isLoaded = true;
    public bool isBeingWorkedOn { get; set; } = false;
    public float fireForce = 100;
    public int cannonSetNum;
    public float rotationSpeed = 5f;
 
    private float minVerticalAngle = -200f;
    private float maxVerticalAngle = 10f;
    private Transform cannonTransform;
    private float minHorizontalAngle = -20f;
    private float maxHorizontalAngle = 20f;

    private void Start()
    {
        cannonLine = gameObject.GetComponentInChildren<CannonLine>();
        gunAnim = gameObject.GetComponent<Animator>();
        cannonAudioSource = gameObject.GetComponentInChildren<AudioSource>();
        haloControl = gameObject.GetComponentInChildren<HaloController>();
        cannonTransform = transform.Find("Cannon");
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

            SetLineActivity(false);
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

    //load the cannon
    public void LoadGun()
    {
        if (isLoaded == false) {
            gunAnim.SetBool("isLoaded", true);
            haloControl.SetHalo(false);
            isLoaded = true;
            isBeingWorkedOn = false;
        }
        //Debug.Log("reload gun");
    }


    public void AdjustVerticalAngle(int input)
    {
        if (input != 0)
        {
            // Normalize current angle to -180 to 180
            float currentAngle = Mathf.Repeat(rotationPoint.localEulerAngles.x + 180f, 360f) - 180f;

            // Adjust and clamp the angle
            float newAngle = Mathf.Clamp(currentAngle - input * rotationSpeed * Time.deltaTime, minVerticalAngle, maxVerticalAngle);

            // Apply the clamped angle
            rotationPoint.localEulerAngles = new Vector3(newAngle, rotationPoint.localEulerAngles.y, rotationPoint.localEulerAngles.z);

            // Debug log for diagnostics
            //Debug.Log($"Current angle: {currentAngle}, New angle: {newAngle}, Input: {input}, Min: {minVerticalAngle}, Max: {maxVerticalAngle}");
        }
    }


    // Adjust horizontal rotation directly
    public void AdjustHorizontalAngle(float input)
    {
        if (input != 0)
        {

            horizontalGunAngle = Mathf.Clamp(horizontalGunAngle + input * rotationSpeed * Time.deltaTime, minHorizontalAngle, maxHorizontalAngle);

            cannonTransform.localEulerAngles = new Vector3(cannonTransform.localEulerAngles.x, horizontalGunAngle, cannonTransform.localEulerAngles.z);
        }
    }

}
