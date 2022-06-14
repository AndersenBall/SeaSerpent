using ECM.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPositionInterface : MonoBehaviour
{
    private Rigidbody rigidbod;
    private BaseFirstPersonController firstPersonController;
   

    private void Start()
    {
        firstPersonController = gameObject.GetComponent<BaseFirstPersonController>();
        rigidbod = gameObject.GetComponent<Rigidbody>();
    }

    public void SetFirstPerControllerActive(bool active)
    {
        if (active) {          
            firstPersonController.enabled = true;
            firstPersonController.movement.Freeze(false);

        }
        else {
            firstPersonController.movement.Freeze(true);
            firstPersonController.enabled = false;
            
        }
    }
  
    public Vector3 GetLocalPosition()
    {
        return transform.localPosition;
    }
    public Vector3 GetRotation()
    {
        return transform.eulerAngles;
    }

    public Vector3 GetVelocity()
    {
        return rigidbod.velocity;
    }

}
