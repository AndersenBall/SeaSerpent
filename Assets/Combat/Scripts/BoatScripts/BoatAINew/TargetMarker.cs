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
        MoveToTarget();
    }
    

    #endregion

    private void MoveToTarget()
    {
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            targetPosition.y += 15f;
            transform.position = targetPosition;
        }   
    }

    public void SetVisible(bool visible)
    {
        foreach (var r in _renderers)
        {
            if (r) r.enabled = visible;
        }
    }

    public void SetPosition(Vector3 worldPos)
    {
        target = null;
        transform.position = worldPos;
    }

    public void FollowTarget(GameObject obj)
    {
        target = obj;
        MoveToTarget();
    }

    #region waypoints
    public void AddWaypoint(Vector3 position)
    {
        waypoints.Enqueue(position);
    }

    /// <summary>
    /// Advance to next queued waypoint (if any) and position THIS marker there.
    /// Returns true if a waypoint was consumed.
    /// </summary>
    public bool AdvanceToNextWaypoint()
    {
        if (waypoints.Count > 0)
        {
            SetPosition(waypoints.Dequeue());
            return true;
        }
        return false;
    }

    public void ClearWaypoints() => waypoints.Clear();

    public bool HasWaypoints => waypoints.Count > 0;

    public Vector3? PeekNextWaypoint()
    {
        return waypoints.Count > 0 ? waypoints.Peek() : (Vector3?)null;
    }
    #endregion
    
    
}

