using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonLine : MonoBehaviour
{
    public CannonInterface cannonSet;
    public LineRenderer lineRender;
    public float angleOffset;
    public int numberOfPoints;
    public Transform barrel;
    // Start is called before the first frame update
    void Start()
    {
        lineRender = gameObject.GetComponent<LineRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("y")) {
            cannonSet.Fire();
        }
        if (Input.GetKeyDown("r")) {
            cannonSet.LoadGun();
        }
        
        UpdateLineRender();
        
    }
    public void SetActive(bool enabled) {
        lineRender.enabled = enabled;
    }
    public void RotationOffset(float angle) {
        if (angle == 0) {
            angleOffset = 0;
            return;
        }
        angleOffset -= angle;
    }


    public void UpdateLineRender() {
        if (angleOffset != 0) {
            lineRender.material.color = Color.white;
        }
        else {
            lineRender.material.color = Color.red;
        }

        lineRender.positionCount = numberOfPoints;
        Vector3 cannonDirection = barrel.transform.forward;
        cannonDirection = Quaternion.AngleAxis(angleOffset,barrel.transform.right) * cannonDirection;
        float fireSpeed = cannonSet.GetFireSpeed();

        float vZ = cannonDirection.z * fireSpeed;
        float vX = cannonDirection.x * fireSpeed;
        float vY = cannonDirection.y * fireSpeed;
        float aY = Physics.gravity.y;
        
        //Debug.Log("aY:" + aY + " vY:" + vY + " vX" + vX + " vZ:" + vZ);

        for (int i = 0; i < numberOfPoints; i++) {
            float pX = vX * (i / 10f);
            float pZ = vZ * (i / 10f);
            float pY = vY * (i / 10f) + aY / 2f * (i / 10f) * (i / 10f);
            
            lineRender.SetPosition(i,new Vector3(pX,pY,pZ  ) + barrel.position);
            
        }
    }
}
