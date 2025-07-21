using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementOnBoat : MonoBehaviour
{

    public CharacterPositionInterface characterPositionInterface;

    
    // Update is called once per frame
    void Update()
    {
        
        UpdateLocalPosition();
    }
    
 
    //moves the object to the location of the nav mesh object, simulating movement.
    public void UpdateLocalPosition()
    {
        transform.localPosition = characterPositionInterface.GetLocalPosition();
        transform.localEulerAngles = characterPositionInterface.GetRotation();
    }

    //sets destination to move to. Converts world to local based on boat parent
    
}
