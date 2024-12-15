using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using System.Drawing;

public class CannonInterface : MonoBehaviour
{
    public ParticleSystem muzzleFlash;
    public Transform firePoint;
    public Transform rotationPoint;

    public GameObject bulletPrefab;

    private Animator gunAnim;
    private AudioSource cannonAudioSource;
    private HaloController haloControl;
    private CannonLine cannonLine;
    
    private float gunAngle=0;
    private float _wantedVerticalAngle = 0;
    private float _wantedHorizontalAngle = 0;
    public float WantedVerticalAngle
    {
        get => -_wantedVerticalAngle; 
        set => _wantedVerticalAngle = Mathf.Clamp(-value, _minVerticalAngle, _maxVerticalAngle); 
    }

   
    public float WantedHorizontalAngle
    {
        get => _wantedHorizontalAngle; 
        set => _wantedHorizontalAngle = Mathf.Clamp(value, minHorizontalAngle, maxHorizontalAngle); 
    }
    private float horizontalGunAngle = 0;

    [SerializeField]
    private float cannonVariance = 1.2f;
    private bool isLoaded = true;
    public bool isBeingWorkedOn { get; set; } = false;
    public float fireForce = 100;
    public int cannonSetNum;
    public float rotationSpeed = 5f;

    [SerializeField]
    private float _minVerticalAngle = -20f;
    [SerializeField]
    private float _maxVerticalAngle = 10f;
    //invert values as the barrel is messed up
    public float MinVerticalAngle
    {
        get => -_minVerticalAngle; 
        set => _minVerticalAngle = -value; 
    }

    public float MaxVerticalAngle
    {
        get => -_maxVerticalAngle; 
        set => _maxVerticalAngle = -value;
    }
    private Transform cannonTransform;
    private float minHorizontalAngle = -25f;
    private float maxHorizontalAngle = 25f;

    private void Start()
    {
        cannonLine = gameObject.GetComponentInChildren<CannonLine>();
        gunAnim = gameObject.GetComponent<Animator>();
        cannonAudioSource = gameObject.GetComponentInChildren<AudioSource>();
        haloControl = gameObject.GetComponentInChildren<HaloController>();
        cannonTransform = transform.Find("Cannon");
    }
    private void Update()
    {
        UpdateCannonRotation();
    }

    private void UpdateCannonRotation()
    {
        // Adjust vertical rotation
        float currentVerticalAngle = Mathf.Repeat(rotationPoint.localEulerAngles.x + 180f, 360f) - 180f;
        if (Mathf.Abs(currentVerticalAngle - _wantedVerticalAngle) > 0.01f) // Avoid unnecessary updates for tiny differences
        {
            float newVerticalAngle = Mathf.MoveTowards(currentVerticalAngle, _wantedVerticalAngle, rotationSpeed * Time.deltaTime);
            newVerticalAngle = Mathf.Clamp(newVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
            rotationPoint.localEulerAngles = new Vector3(newVerticalAngle, rotationPoint.localEulerAngles.y, rotationPoint.localEulerAngles.z);
        }

        // Adjust horizontal rotation
        if (Mathf.Abs(horizontalGunAngle - _wantedHorizontalAngle) > 0.01f)
        {
            horizontalGunAngle = Mathf.MoveTowards(horizontalGunAngle, _wantedHorizontalAngle, rotationSpeed * Time.deltaTime);
            horizontalGunAngle = Mathf.Clamp(horizontalGunAngle, minHorizontalAngle, maxHorizontalAngle);
            cannonTransform.localEulerAngles = new Vector3(cannonTransform.localEulerAngles.x, horizontalGunAngle, cannonTransform.localEulerAngles.z);
        }
    }

    #region legacy
    public bool GetNeedsRotation() {
        if (_wantedVerticalAngle == gunAngle)
            return false;
        else
            return true;
    }
    public void RotateBarrel() // amount to rotate the gun
    {
        cannonLine.RotationOffset(0);
        float rotationAmount = _wantedVerticalAngle - gunAngle;
        gunAngle = _wantedVerticalAngle;
        rotationPoint.Rotate(rotationAmount, 0, 0);
    }
    public void UpdateWantedBarrelAngle(float ang) {//set amount you want the cannon to be at, for computer.
        
        if (_wantedVerticalAngle - ang > -25 && _wantedVerticalAngle - ang < 10) {
            _wantedVerticalAngle -= ang;
            cannonLine.RotationOffset(ang);
        }
    }
    public void SetWantedBarrelAngle(float ang)
    {//set amount you want the cannon to be at
        
        float prevWanted = _wantedVerticalAngle;
        _wantedVerticalAngle = -Mathf.Clamp(ang, -10, 25); ;
        cannonLine.RotationOffset(prevWanted-_wantedVerticalAngle);
    }
    #endregion


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
            GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position , Quaternion.identity);
            Rigidbody bulletRigidBody = bulletObject.GetComponent<Rigidbody>();

            //sets direction that object should be facing based off guns directon and fires it
            bulletObject.transform.forward = firePoint.forward;
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
            bulletRigidBody.velocity = Quaternion.AngleAxis(randNormalX, Vector3.up)* Quaternion.AngleAxis(randNormalY,Vector3.right) * firePoint.forward * fireForce;

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
            float newAngle = Mathf.Clamp(currentAngle - input * rotationSpeed * Time.deltaTime, _minVerticalAngle, _maxVerticalAngle);

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
