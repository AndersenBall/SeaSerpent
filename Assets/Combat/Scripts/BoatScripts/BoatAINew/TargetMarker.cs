// TargetMarker.cs
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    [SerializeField] private Transform visualsRoot;

    private Renderer[] _renderers;

    private void Awake()
    {
        if (!visualsRoot && transform.childCount > 0)
            visualsRoot = transform.GetChild(0);

        _renderers = (visualsRoot ? visualsRoot : transform).GetComponentsInChildren<Renderer>(true);
    }

    public void SetVisible(bool visible)
    {
        if (visualsRoot)
        {
            visualsRoot.gameObject.SetActive(visible);
        }
        else
        {
            foreach (var r in _renderers) r.enabled = visible;
        }
    }

    public void SetPosition(Vector3 worldPos)
    {
        transform.position = worldPos;
    }

    // Optional: call this if you want the flag to face the camera
    public void FaceCamera(Camera cam)
    {
        if (!cam || !visualsRoot) return;
        visualsRoot.LookAt(cam.transform, Vector3.up);
    }
}
