using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTestAdjust : MonoBehaviour
{
    public int distance;
    public CannonInterface cannon;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        if (Input.GetKeyDown("u")) {
            Debug.Log(PredictAngle(distance));
            //cannon.SetWantedBarrelAngle(PredictAngle(distance));
            cannon.RotateBarrel();
        }
        if (Input.GetKeyDown("f")) {
            cannon.Fire(0);

        }
        if (Input.GetKeyDown("r")) {
            cannon.LoadGun();
        }
    }

    // Update is called once per frame
    public float PredictAngle(float distance) {
        float value = Mathf.Asin(distance / 4077.47f) / 2 * (180 / Mathf.PI);
        if (float.IsNaN(value)) {
            return 45f;
        }
        else {
            return value;
        }
    }
}
