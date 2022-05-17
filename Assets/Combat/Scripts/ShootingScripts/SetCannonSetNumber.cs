using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCannonSetNumber : MonoBehaviour
{
    public int cannonSetNumber = 0;
    void Start()
    {
        CannonInterface[] cannonInterfaces = gameObject.GetComponentsInChildren<CannonInterface>();

        foreach (CannonInterface cannonInt in cannonInterfaces) {
            cannonInt.setCannonSetNum(cannonSetNumber);
        
        }
    }

    
}
