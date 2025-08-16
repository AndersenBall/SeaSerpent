using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine;

using System.Collections;
using UnityEngine;

using UnityEngine;

using UnityEngine;

public class MastControls : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private Vector3 globalWindDirection = Vector3.forward; 
    public bool IsInSweetSpot { get; private set; } 
    public bool IsBeyondMaxRotation { get; private set; }
    public bool sweetSpotInMaxRotation{ get; private set; }


    [SerializeField]
    private float rotationSpeed = 10f; 

    [SerializeField]
    private float minHorizontalAngle = -80f;
    [SerializeField]
    private float maxHorizontalAngle = 80f; 

    private float _targetAngle = 0f; 
    private bool _isPlayerControlling = false;
    private bool _isManned = false; 
    #endregion

    #region Unity Methods
    void FixedUpdate()
    {
        if (_isPlayerControlling)
        {
            float input = Input.GetAxis("Horizontal");
            if (input != 0)
            {
                AdjustHorizontalAngle(input);
            }
        }

        if (_isManned || _isPlayerControlling)
        {
            UpdateSweetSpotStatus();
            RotateMast();
        }


    }
    #endregion

    #region mast controls 
    public void SetRotation(float angle) 
    {
        _targetAngle = Mathf.Clamp(angle, minHorizontalAngle, maxHorizontalAngle);
        _isPlayerControlling = false;
    }

    public void EnablePlayerControl() 
    {
        _isPlayerControlling = true;
    }

    private void AdjustHorizontalAngle(float input)
    {
        _targetAngle = Mathf.Clamp(_targetAngle + input * rotationSpeed * Time.deltaTime, minHorizontalAngle, maxHorizontalAngle);
    }

    private void RotateMast()
    {
        float currentAngle = transform.localEulerAngles.y;
        if (currentAngle > 180f) currentAngle -= 360f; 

        float newAngle = Mathf.MoveTowards(currentAngle, _targetAngle, rotationSpeed * Time.deltaTime);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newAngle, transform.localEulerAngles.z);
    }
    #endregion
    
    #region wind boost 
    private void UpdateSweetSpotStatus()
    {
        Vector3 mastForward = transform.forward;
        float angleDifference = Vector3.Angle(mastForward, globalWindDirection);
        
        float windRelativeAngle = Vector3.SignedAngle(transform.forward, globalWindDirection, Vector3.up);
        
        IsBeyondMaxRotation = windRelativeAngle < minHorizontalAngle || windRelativeAngle > maxHorizontalAngle;
        
        float tolerance = 10f; 
        IsInSweetSpot = angleDifference <= tolerance;

        // Check if mast is at max rotation and wind is beyond bounds on left or right
        if (IsBeyondMaxRotation)
        {
            if (windRelativeAngle < minHorizontalAngle && _targetAngle == minHorizontalAngle)
            {
                sweetSpotInMaxRotation = true; // Wind beyond left, mast at max left
            }
            else if (windRelativeAngle > maxHorizontalAngle && _targetAngle == maxHorizontalAngle)
            {
                sweetSpotInMaxRotation = true; // Wind beyond right, mast at max right
            }
            else
            {
                sweetSpotInMaxRotation = false;
            }
        }
        else
        {
            sweetSpotInMaxRotation = false; // Reset if wind is not beyond limits
        }
    }




    #endregion
}