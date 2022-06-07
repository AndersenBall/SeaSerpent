using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTimeScale : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("=")) {
            if(Time.timeScale!= 0)
                Time.timeScale += .5f;
        }
        if (Input.GetKeyDown("-")) {
            if (!(Time.timeScale-.5 <= 0f))
                Time.timeScale -= .5f;
        }
    }
}
