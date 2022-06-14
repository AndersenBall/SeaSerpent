using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementOnBoat : MonoBehaviour
{

    public CharacterPositionInterface characterPositionInterface;
    public BoatControls boatControl;

    public bool isFirstPerson = true;

    //boat controls variables
    float sideways = 0;
    float throtal = 0;
    
    void Start()
    {
        boatControl = gameObject.GetComponentInParent<BoatControls>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstPerson == false) {
            sideways = (Input.GetKey(KeyCode.A) ? -1f : 0f) +
                        (Input.GetKey(KeyCode.D) ? 1f : 0f);
            boatControl.SetTurn(sideways);
            if (Input.GetKeyDown(KeyCode.W)) {
                throtal = throtal + .1f;
                throtal = Mathf.Clamp(throtal, -1, 1);
                boatControl.SetForward(throtal);
            }
            else if (Input.GetKeyDown(KeyCode.S)) {
                throtal = throtal - .1f;
                throtal = Mathf.Clamp(throtal, -1, 1);
                boatControl.SetForward(throtal);
            }
                        

        }
        UpdateLocalPosition();
    }
    
    public void SetIsActive(bool active)
    {
        isFirstPerson = active;
        characterPositionInterface.SetFirstPerControllerActive(active);
    }
    public void ActiveBoatControls(BoatControls b)
    {
        boatControl = b;
        isFirstPerson = false;
        characterPositionInterface.SetFirstPerControllerActive(false);
    }

    //moves the object to the location of the nav mesh object, simulating movement.
    public void UpdateLocalPosition()
    {
        transform.localPosition = characterPositionInterface.GetLocalPosition();
        transform.localEulerAngles = characterPositionInterface.GetRotation();
    }

    //sets destination to move to. Converts world to local based on boat parent
    
}
