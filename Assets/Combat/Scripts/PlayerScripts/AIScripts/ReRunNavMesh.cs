using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReRunNavMesh : MonoBehaviour
{
    public NavMeshSurface surface;
    // Start is called before the first frame update
    void Start()
    {
        surface = gameObject.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
    private void Update()
    {
        if (Input.GetKeyDown("9")) {
            Debug.Log("Re run nav mesh");
            surface = gameObject.GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();
        }
    }

    public void RunNavMesh() {
        surface = gameObject.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

}
