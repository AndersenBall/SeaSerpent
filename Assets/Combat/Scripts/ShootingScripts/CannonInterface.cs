using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using System.Drawing;
using MapMode.Scripts.DataTypes.boatComponents.Cannons;

public class CannonInterface : MonoBehaviour
{
    public CannonType cannonType;
    public ParticleSystem muzzleFlash;
    public Transform firePoint;
    public Transform rotationPoint;

    public GameObject bulletPrefab;

    private Animator gunAnim;
    private AudioSource cannonAudioSource;
    private HaloController haloControl;
    private CannonLine cannonLine;
    
    private float gunAngle=0;
    [SerializeField]
    private float _wantedVerticalAngle = 0;
    [SerializeField]
    private float _wantedHorizontalAngle = 0;

    public int baseCannonDamage = 0;
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
    [SerializeField]
    private float _currentHorizontalAngle = 0;
    public float currentHorizontalAngle { get =>_currentHorizontalAngle; set =>_currentHorizontalAngle = value; }

    [SerializeField]
    public float _currentVerticalAngle = 0;

    public float currentVerticalAngle { get => _currentVerticalAngle; set => _currentVerticalAngle = value; }

    [SerializeField]
    private float cannonVariance = 1.2f;
    private bool isLoaded = true;
    [SerializeField]
    private bool _isBeingWorkedOn = false;
    public bool isBeingWorkedOn { get =>_isBeingWorkedOn; set => _isBeingWorkedOn = value; }
    [SerializeField]
    private bool _isManned = false;
    public bool isManned
    {
        get => _isManned;
        set => _isManned = value;
    }

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
    [SerializeField]
    private float minHorizontalAngle = -25f;
    [SerializeField]
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
        if (_isManned)
        {
            UpdateCannonRotation();
        }
        
    }

    private void UpdateCannonRotation()
    {
        // Adjust vertical rotation
        currentVerticalAngle = Mathf.Repeat(rotationPoint.localEulerAngles.x + 180f, 360f) - 180f;
        if (Mathf.Abs(currentVerticalAngle - _wantedVerticalAngle) > 0.01f) // Avoid unnecessary updates for tiny differences
        {
            float newVerticalAngle = Mathf.MoveTowards(currentVerticalAngle, _wantedVerticalAngle, rotationSpeed * Time.deltaTime);
            newVerticalAngle = Mathf.Clamp(newVerticalAngle, _minVerticalAngle, _maxVerticalAngle);
            rotationPoint.localEulerAngles = new Vector3(newVerticalAngle, rotationPoint.localEulerAngles.y, rotationPoint.localEulerAngles.z);
        }

        // Adjust horizontal rotation
        if (Mathf.Abs(currentHorizontalAngle - _wantedHorizontalAngle) > 0.01f)
        {
            currentHorizontalAngle = Mathf.MoveTowards(currentHorizontalAngle, _wantedHorizontalAngle, rotationSpeed * Time.deltaTime);
            currentHorizontalAngle = Mathf.Clamp(currentHorizontalAngle, minHorizontalAngle, maxHorizontalAngle);
            cannonTransform.localEulerAngles = new Vector3(cannonTransform.localEulerAngles.x, currentHorizontalAngle, cannonTransform.localEulerAngles.z);
        }
    }

    #region legacy
    public bool GetNeedsRotation() {
        if ((Mathf.Abs(currentHorizontalAngle + (-_wantedHorizontalAngle)) < 0.01f) && (Mathf.Abs(currentVerticalAngle + (-_wantedVerticalAngle)) < 0.01f))
            return false;
        else
            return true;
    }
    public void RotateBarrel() // amount to rotate the gun
    {
        return;
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
            BombStart bombStart = bulletObject.GetComponent<BombStart>();
            if (bombStart != null)
            {
                bombStart.damageDeal += baseCannonDamage;
            }
            
            bulletObject.transform.forward = firePoint.forward;
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

            //SetLineActivity(false);
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

            float currentAngle = Mathf.Repeat(rotationPoint.localEulerAngles.x + 180f, 360f) - 180f;

            WantedVerticalAngle = -currentAngle + input * rotationSpeed * Time.deltaTime;

        }
    }

    public void AdjustHorizontalAngle(float input)
    {
        if (input != 0)
        {
            WantedHorizontalAngle = currentHorizontalAngle + input * rotationSpeed * Time.deltaTime;

        }
    }

    public void WorkOnCannon()
    {
        SetLineActivity(true);
        isManned = true;
    }

    public void StopWorkingOnCannon()
    {
        SetLineActivity(false);
        isManned = false;
        isBeingWorkedOn = false;
    }

    public void SetCannonValues(Cannon cannon)
    {
        cannonType = cannon.CannonType;
        baseCannonDamage = cannon.Damage;
        fireForce = cannon.ShotPower;
        MinVerticalAngle = cannon.MaxVerticalAngle;
        MaxVerticalAngle = cannon.MinVerticalAngle;
        minHorizontalAngle = -cannon.MaxHorizontalAngle;
        maxHorizontalAngle = cannon.MaxHorizontalAngle;
        rotationSpeed = cannon.TurnSpeed;
        cannonVariance = cannon.Variance;
    }
}
