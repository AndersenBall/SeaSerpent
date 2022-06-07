using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public LayerMask layer;
    Vector3 newPosition;
    void Start()
    {
        newPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit,layer)) {
                newPosition = new Vector3(hit.point.x,0,hit.point.z);
                transform.position = newPosition;
            }
        }
    }
}
