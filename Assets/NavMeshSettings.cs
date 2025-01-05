using UnityEngine;
using UnityEngine.AI;

public class NavMeshSettings : MonoBehaviour
{
    void Awake()
    {
        // Increase the global pathfinding iterations per frame
        NavMesh.pathfindingIterationsPerFrame = 1000; 
    }
}
