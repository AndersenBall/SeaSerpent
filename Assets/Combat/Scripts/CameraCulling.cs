using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent<Camera>();
        float[] distances = new float[32];
        distances[18] = 5;
        cam.layerCullDistances = distances;
    }

    // Update is called once per frame
   
}
