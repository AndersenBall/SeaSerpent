using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    public Camera cam;

    public NavMeshUnitInterface navMeshInterface;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLocalPosition();
    }

    //moves the object to the location of the nav mesh object, simulating movement.
    public void UpdateLocalPosition() {
        transform.localPosition = navMeshInterface.GetLocalPosition();
        transform.localEulerAngles = navMeshInterface.GetRotation();
    }

    //sets destination to move to. Converts world to local based on boat parent
    public void SetDestination(Vector3 destination) {
        navMeshInterface.SetDestination(transform.parent.InverseTransformPoint(destination));
    }

    public NavMeshUnitInterface GetNavMeshInterface() {
        return navMeshInterface;
    }

}
