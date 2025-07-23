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

    public void SetUpCharacterPositionInterface(BoatControls b)
    {
        //TODO delete old character ai
        //TODO Spawn new ai and set character interface
        //OR position new one in the new ai boat and set its transform there based on
        //relative position to the new boat we just entered
    }
}
