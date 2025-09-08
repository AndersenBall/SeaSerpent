// TargetMarker.cs

using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    #region  variables 
    private Renderer[] _renderers;
    private GameObject target;
    private Queue<Vector3> waypoints = new Queue<Vector3>();
    

    #endregion

    #region  monobehaviors
    
    private void Awake()
    {
        // Cache all renderers under this prefab (including children)
        _renderers = GetComponentsInChildren<Renderer>(true);
    }
    private void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            targetPosition.y += 15f;
            transform.position = targetPosition;
        }
    }
    

    #endregion
    

    public void SetVisible(bool visible)
    {
        foreach (var r in _renderers)
        {
            if (r) r.enabled = visible;
        }
    }

    public void SetPosition(Vector3 worldPos)
    {
        transform.position = worldPos;
    }

    public void FollowTarget(GameObject obj)
    {
        target = obj;
    }

    public void StopFollowingTarget()
    {
        target = null;
    }

    public void AddWaypoint(Vector3 position)
    {
        waypoints.Enqueue(position);
    }

    public bool CheckpointReached()
    {
        if(waypoints.Count > 0)
        {
            if(target != null)
            {
                target.transform.position = waypoints.Dequeue();
                return true;
            }
        }
        return false;
    }

    public void ClearWaypoints()
    {
        waypoints.Clear();
    }
    
    
}

