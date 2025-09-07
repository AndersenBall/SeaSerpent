// TargetMarker.cs
using UnityEngine;

using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    private Renderer[] _renderers;

    private void Awake()
    {
        // Cache all renderers under this prefab (including children)
        _renderers = GetComponentsInChildren<Renderer>(true);
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
        transform.position = worldPos;
    }

    // Optional: make the marker face the camera
    public void FaceCamera(Camera cam)
    {
        if (!cam) return;
        transform.LookAt(cam.transform, Vector3.up);
    }
}

