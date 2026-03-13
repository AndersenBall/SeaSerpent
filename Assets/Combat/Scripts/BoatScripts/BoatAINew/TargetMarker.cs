// TargetMarker.cs

using System.Collections.Generic;
using UnityEngine;


public enum WaypointKind { Position, Follow }

[System.Serializable]
public class Waypoint
{
    
    public WaypointKind Kind;
    public Vector3 Position;      // used when Kind == Position
    public Transform Target;      // used when Kind == Follow
    public bool IsEnemy;          // for tint/logic
    public float MinDistance;     // arrival/engage distance hint (optional)

    public bool IsValid => Kind == WaypointKind.Position || Target != null;

    public Vector3 GetWorldPos()
    {
        if (Kind == WaypointKind.Position)
            return Position;
        if (Kind == WaypointKind.Follow && Target != null)
            return Target.position;
        return Position;
    }

}


public class TargetMarker : MonoBehaviour
{
    #region  variables 
    
    #if UNITY_EDITOR
    [Header("DEBUG (Read Only)")]
    [SerializeField] private Waypoint[] _debugWaypoints;
    #endif
    
    [Header("Visuals")]
    [Tooltip("Small prefab (quad/disc/flag). It should face up; scale it in the prefab.")]
    [SerializeField] private GameObject markerPrefab;
    [Tooltip("Max number of markers we ever show (pool size).")]
    [SerializeField] private int poolSize = 12;
    [Tooltip("Lift markers slightly to avoid z-fighting with ground/water.")]
    [SerializeField] private float yOffset = 0.05f;
    [Tooltip("Tint for the NEXT waypoint.")]
    [SerializeField] private Color nextColor = Color.white;
    [Tooltip("Tint for the remaining waypoints.")]
    [SerializeField] private Color queuedColor = Color.gray;
    [SerializeField] private Color enemyColor = new Color(1f, 0.35f, 0.35f);
    [SerializeField] private Color allyColor  = new Color(0.55f, 0.8f, 1f);
    
    private Queue<Waypoint> waypoints = new Queue<Waypoint>();
    
    private readonly List<GameObject> pool = new();
    [SerializeField] private bool visualsVisible = true;
    private static Transform s_worldRoot;

    #endregion

    #region  monobehaviors
    
    private void Start()
    {
        var parent = GetWorldRoot();

        if (markerPrefab == null)
        {
            Debug.LogWarning($"{nameof(TargetMarker)} on {name}: markerPrefab not set. Only queue will work; no visuals will render.");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            var go = Instantiate(markerPrefab, parent);
            go.SetActive(false);
            pool.Add(go);
        }
        
    }
    
    private void OnDestroy()
    {
        for (int i = 0; i < pool.Count; i++)
            if (pool[i]) Destroy(pool[i]);
    }


    #endregion
    
    #region waypoints
    public void AddWaypoint(Vector3 worldPos, float minDistance = 0f, bool isEnemy = false)
    {
        waypoints.Enqueue(new Waypoint {
            Kind = WaypointKind.Position,
            Position = worldPos,
            IsEnemy = isEnemy,
            MinDistance = minDistance
        });
        #if UNITY_EDITOR
                SyncDebugList();
        #endif
        if (visualsVisible) RefreshVisuals();
    }

    /// Follow a moving Transform (ally or enemy).
    public void AddFollowWaypoint(Transform target, bool isEnemy, float minDistance = 0f)
    {
        if (target == null) return;
        waypoints.Enqueue(new Waypoint {
            Kind = WaypointKind.Follow,
            Target = target,
            IsEnemy = isEnemy,
            MinDistance = minDistance
        });
        #if UNITY_EDITOR
            SyncDebugList();
        #endif
        if (visualsVisible) RefreshVisuals();
    }

    public void ClearWaypoints()
    {
        waypoints.Clear();
        #if UNITY_EDITOR
            SyncDebugList();
        #endif
        if (visualsVisible) RefreshVisuals();
    }

    /// <summary>
    /// Advance to next queued waypoint (if any) and position THIS marker there.
    /// </summary>
    public bool AdvanceToNextWaypoint()
    {
        if (waypoints.Count > 0)
        {
            waypoints.Dequeue();
            #if UNITY_EDITOR
                SyncDebugList();
            #endif
            if (visualsVisible) RefreshVisuals();
            return true;
        }
        return false;
    }

    public bool HasWaypoints => waypoints.Count > 0;

    public Waypoint? PeekNextWayPoint()
    {
        if (waypoints.Count == 0) return null;
        var wp = waypoints.Peek();
        return wp;
    }
    #endregion
    
    #region visual
    public void SetVisible(bool visible)
    {
        visualsVisible = visible;
        if (visible)
        {
            RefreshVisuals();
        }
        else{
            foreach (var t in pool)
                if (t) t.SetActive(false);
        }
    }
    
    private void RefreshVisuals()
    {
        if (pool.Count == 0 || !markerPrefab) return;

        // draw at *current* positions (so Follow targets move the markers)
        int i = 0;
        foreach (var wp in waypoints)
        {
            if (i >= pool.Count) break;

            var pos = wp.GetWorldPos();
            var inst = pool[i++];
            inst.transform.position = new Vector3(pos.x, pos.y + yOffset, pos.z);
            inst.SetActive(true);

            if (inst.TryGetComponent<Renderer>(out var r))
            {
                // first marker = "next"
                var baseColor = (i == 1) ? nextColor : queuedColor;

                // overlay ally/enemy tint if this is a Follow target
                if (wp.Kind == WaypointKind.Follow)
                    baseColor = wp.IsEnemy ? enemyColor : allyColor;

                r.material.color = baseColor;
            }
        }

        for (; i < pool.Count; i++)
            pool[i].SetActive(false);
    }
    
    private Transform GetWorldRoot()
    {
        if (s_worldRoot != null) return s_worldRoot;

        var existing = GameObject.Find("WaypointMarkersRoot");
        if (existing != null) return s_worldRoot = existing.transform;

        var go = new GameObject("WaypointMarkersRoot");
        return s_worldRoot = go.transform;
    }
    #endregion
    
    #region editor
    #if UNITY_EDITOR
        private void SyncDebugList()
        {
            _debugWaypoints = waypoints.ToArray();
        }
    #endif

    #endregion
}

