using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookPlayer : MonoBehaviour
{
    public float mouseSensitivity = 50f;
    private float mouseY;
    private float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(xRotation, 0f, 0f), 100 * Time.deltaTime);
        
    }
}
