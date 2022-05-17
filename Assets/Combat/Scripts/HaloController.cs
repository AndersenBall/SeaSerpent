using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloController : MonoBehaviour
{
    // Start is called before the first frame update

    private Behaviour haloEffect;
    void Start()
    {
        haloEffect = (Behaviour)GetComponent("Halo");
    }

    // Update is called once per frame
    public void SetHalo(bool lightBool) {
        haloEffect.enabled = lightBool;
    }
}
