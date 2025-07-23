using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamController : MonoBehaviour
{
    public void SetUPCamera(Transform boat)
    {
        this.transform.parent = boat.transform;
        this.transform.localPosition = new Vector3(0, 67f, 0);
    }
}
