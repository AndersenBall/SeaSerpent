using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCameraControl : MonoBehaviour
{
    [SerializeField]
    private Transform cannonBarrel; 

    private Camera cannonCamera; 
    [SerializeField]
    private Vector3 cameraOffset = new Vector3(0f, 1f, -3f); 
    [SerializeField]
    private Vector3 cameraRotation = new Vector3(10f, 0f, 0f);
    private void Awake()
    {
        // Find the camera component on this GameObject
        cannonCamera = GetComponent<Camera>();

        if (cannonCamera == null)
        {
            Debug.LogError("CannonCameraController: No Camera component found on this GameObject.");
        }
    }



    public void EnterCannonMode(Transform cannon)
    {
        this.cannonBarrel = cannon;
        // Parent the camera to the cannon barrel
        cannonCamera.transform.SetParent(cannonBarrel);

        // Position the camera behind the barrel
        cannonCamera.transform.localPosition = cameraOffset;

        // Optionally adjust the rotation
        cannonCamera.transform.localRotation = Quaternion.Euler(cameraRotation);

        // Activate the camera
        cannonCamera.enabled = true;

        Debug.Log("Entered cannon mode. Camera repositioned.");
    }

    public void ExitCannonMode()
    {
        // Unparent the camera (optional, depending on your design)
        cannonCamera.transform.SetParent(null);

        // Deactivate the camera
        cannonCamera.enabled = false;

        Debug.Log("Exited cannon mode. Camera deactivated.");
    }
}
