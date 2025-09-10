using System.Collections.Generic;
using UnityEngine;

public class RTSBoxSelection : MonoBehaviour
{
    #region FIELDS
    private Vector2 startScreenPosition; // Start of drag
    private Vector2 endScreenPosition;   // End of drag
    private bool isDragging = false;     // Is the player currently dragging?
    private HashSet<BoatAI> selectedBoats = new HashSet<BoatAI>(); // Selected boats

    private const float YMin = -60f; // Lower Y boundary
    private const float YMax = 100f;  // Upper Y boundary

    private Camera cam; // Reference to the Camera component
    private List<BoatAI> allBoats = new List<BoatAI>();

    #endregion

    #region MONOBEHAVIOUR METHODS

    private void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("RTSBoxSelection: No Camera component found on this GameObject.");
        }
        else if (!cam.orthographic)
        {
            Debug.LogError("RTSBoxSelection: The Camera is not set to Orthographic.");
        }
    }

    private void Start()
    {
        // Find all BoatAI objects tagged "Team1" and layer 12
        foreach (var boat in FindObjectsOfType<BoatAI>())
        {
            if (boat.gameObject.layer == 12)
            {
                allBoats.Add(boat);
            }
        }
    }

    private void Update()
    {
        HandleInput();
        HandleRightClick();
    }
    
    private void OnDisable()
    {
        foreach (var b in selectedBoats){
            SetBoatSelected(b, false);
        }
    }
    
    #endregion

    #region INPUT HANDLING

    private void HandleRightClick()
    {
        // Check if the right mouse button is clicked
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                GameObject hitObject = hit.collider.gameObject;

                // Check if the hit object is an enemy boat
                if (hitObject.layer == 12 && hitObject.CompareTag("Team2"))
                {
                    BoatAI targetEnemy = hitObject.GetComponent<BoatAI>();
                    if (targetEnemy != null)
                    {
                        // Assign the target enemy to all selected boats
                        foreach (var boat in selectedBoats)
                        {
                            boat.targetEnemy = targetEnemy;
                            boat.SetTargetPosition(targetEnemy.gameObject);
                        }

                        Debug.Log($"Target enemy set to {targetEnemy.name} for {selectedBoats.Count} selected boats.");
                    }
                }
                else{
                    foreach (var boat in selectedBoats)
                    {
                        boat.SetTargetPosition(hit.transform.position);
                    }
                }
            }
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Start dragging
            isDragging = true;
            startScreenPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            // Update the drag end position
            endScreenPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // Finish dragging and select objects
            isDragging = false;
            SelectBoatsInBox();
        }
    }

    #endregion

    #region SELECTION LOGIC

    private void SetBoatSelected(BoatAI boat, bool selected)
    {
        if (boat == null) return;
        boat.SetSelected(selected);
        boat.SetTargetMarkerVisible(true);
    }
    
    private void SelectBoatsInBox()
    {

        allBoats.RemoveAll(boat => boat == null || boat.gameObject == null);
        // Convert screen positions to world positions
        Vector3 startWorld = cam.ScreenToWorldPoint(new Vector3(startScreenPosition.x, startScreenPosition.y, cam.nearClipPlane));
        Vector3 endWorld = cam.ScreenToWorldPoint(new Vector3(endScreenPosition.x, endScreenPosition.y, cam.nearClipPlane));

        // Get X and Z bounds from the drag box
        float xMin = Mathf.Min(startWorld.x, endWorld.x);
        float xMax = Mathf.Max(startWorld.x, endWorld.x);
        float zMin = Mathf.Min(startWorld.z, endWorld.z);
        float zMax = Mathf.Max(startWorld.z, endWorld.z);

        // Clear previous selections if Shift is not held
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            foreach (var boat in selectedBoats)
            {
                SetBoatSelected(boat, false); 
            }
            selectedBoats.Clear();
        }

        Bounds selectionBounds = new Bounds();
        selectionBounds.SetMinMax(
            new Vector3(xMin, YMin, zMin),
            new Vector3(xMax, YMax, zMax)
        );

        foreach (var boat in allBoats)
        {
            if (boat.gameObject.CompareTag("Team1"))
            {
                Collider collider = boat.GetComponent<Collider>();
                if (collider != null && selectionBounds.Intersects(collider.bounds))
                {
                    if (selectedBoats.Add(boat)){
                        SetBoatSelected(boat, true);
                    }
                }
            }
        }

        Debug.Log($"Selected {selectedBoats.Count} boats.");
    }

    #endregion

    #region GUI METHODS

    private void OnGUI()
    {
        if (isDragging)
        {
            Rect rect = GetScreenRect(startScreenPosition, endScreenPosition);
            DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    private Rect GetScreenRect(Vector2 screenPoint1, Vector2 screenPoint2)
    {
        screenPoint1.y = Screen.height - screenPoint1.y;
        screenPoint2.y = Screen.height - screenPoint2.y;
        return Rect.MinMaxRect(
            Mathf.Min(screenPoint1.x, screenPoint2.x),
            Mathf.Min(screenPoint1.y, screenPoint2.y),
            Mathf.Max(screenPoint1.x, screenPoint2.x),
            Mathf.Max(screenPoint1.y, screenPoint2.y)
        );
    }

    private void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color); // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color); // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color); // Left
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color); // Right
    }

    #endregion

    #region DEBUGGING

    private void OnDrawGizmos()
    {
        if (!isDragging) return;

        Vector3 startWorld = cam.ScreenToWorldPoint(new Vector3(startScreenPosition.x, startScreenPosition.y, cam.nearClipPlane));
        Vector3 endWorld = cam.ScreenToWorldPoint(new Vector3(endScreenPosition.x, endScreenPosition.y, cam.nearClipPlane));

        float xMin = Mathf.Min(startWorld.x, endWorld.x);
        float xMax = Mathf.Max(startWorld.x, endWorld.x);
        float zMin = Mathf.Min(startWorld.z, endWorld.z);
        float zMax = Mathf.Max(startWorld.z, endWorld.z);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3((xMin + xMax) / 2, (YMin + YMax) / 2, (zMin + zMax) / 2),
            new Vector3(xMax - xMin, YMax - YMin, zMax - zMin)
        );
    }

    #endregion
}
