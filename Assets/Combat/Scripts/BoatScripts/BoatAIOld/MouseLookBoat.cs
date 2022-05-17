using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookBoat : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform lookLocation;
    public Camera cam;
    private float mouseX;
    private float mouseY;
    private float xRotation = 0f;
    private string zoomType = "normal";
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(playerBody.up * mouseX);
        cam.fieldOfView = cam.fieldOfView -Input.GetAxis("Mouse ScrollWheel") * 30;
        mouseSensitivity -= Input.GetAxis("Mouse ScrollWheel")*55f;
        mouseSensitivity = Mathf.Clamp(mouseSensitivity, 1f, 140);
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 4f, 80);
        if (Input.GetMouseButtonDown(1)) {
            if (zoomType != "normal") {
                zoomType = "normal";
                this.transform.localPosition = new Vector3(0, 0, -40);
                lookLocation.localPosition = new Vector3(0, 17, -17);
                cam.fieldOfView = 60;
                mouseSensitivity = 100;
               
            }
            else {
                if (Vector3.Dot(playerBody.right, lookLocation.forward) < -.3f) {
                    zoomType = "right";
                    this.transform.localPosition = new Vector3(0, 0, -10);
                    lookLocation.localPosition = new Vector3(10, 10, 6);
                }
                if (Vector3.Dot(playerBody.right, lookLocation.forward) > .3f) {
                    zoomType = "right";
                    this.transform.localPosition = new Vector3(0, 0, -10);
                    lookLocation.localPosition = new Vector3(-10, 10, 6);
                }
                //Debug.Log("zoomtype: " + zoomType + "looking dot: " + Vector3.Dot(playerBody.right, lookLocation.forward));

            }
        }
    }
}
