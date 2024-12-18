using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceAtCamera : MonoBehaviour
{
    private Transform currentCameraTransform;

    private void Start()
    {
        UpdateCurrentCamera();
        gameObject.layer = 19;
    }

    private void LateUpdate()
    {
        if (currentCameraTransform != null && currentCameraTransform.GetComponent<Camera>()?.enabled == true)
        {
            transform.LookAt(transform.position + currentCameraTransform.rotation * Vector3.forward,
                             currentCameraTransform.rotation * Vector3.up);
        }
        else
        {
            UpdateCurrentCamera();
        }
    }

    private void UpdateCurrentCamera()
    {
        // Find the active camera in Display 1
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam != null && cam.targetDisplay == 0 && cam.enabled) // Check for not null and enabled
            {
                currentCameraTransform = cam.transform;
                return;
            }
        }

        Debug.LogWarning("No active camera found in Display 1!");
        currentCameraTransform = null; // Set to null if no active camera is found
    }

    public void RefreshCamera()
    {
        UpdateCurrentCamera();
    }
}
