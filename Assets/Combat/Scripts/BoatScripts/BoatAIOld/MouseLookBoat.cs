using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class MouseLookBoat : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform lookLocation;
    public Camera cam;
    public BoatAI boatAi;

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
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, .5f, 80);
        mouseSensitivity = FieldOfViewToZoom(cam.fieldOfView);
        
        

        if (Input.GetMouseButtonDown(1)) {
            if (zoomType != "normal") {
                zoomType = "normal";
                this.transform.localPosition = new Vector3(0, 0, -5);
                lookLocation.localPosition = new Vector3(0, 10  , -20);
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
        if (Input.GetMouseButtonDown(0)) {
            int layerMask = Convert.ToInt32("0000110000100000000", 2);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)){
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                cam.fieldOfView = ZoomToTarget(hit.distance);
                Debug.Log("hit: " + hit.transform.gameObject.layer);
                if (hit.transform.GetComponentInParent<BoatAI>() != null && hit.transform.gameObject.layer == LayerMask.NameToLayer("Team2"))
                {
                    boatAi.targetEnemy = hit.transform.GetComponentInParent<BoatAI>();
                }
                
            } else{
                if (cam.fieldOfView < 70) {
                    cam.fieldOfView = 80;
                }
            }
            
            
        }
    }

    private float ZoomToTarget(float distance) {
        float zoom = (4000 - distance) /(5*Mathf.Pow(distance/100+1,2));
        Debug.Log("Selected Enemy:" +distance +" field of view:" + zoom);
        return zoom;
    }

    private float FieldOfViewToZoom(float field) {
        return field;
    }
}
