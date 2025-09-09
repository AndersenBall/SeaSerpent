namespace Combat.Fishing
{
    // CastGesture.cs
using UnityEngine;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class CastGesture : MonoBehaviour
{
    [Header("Sampling")]
    [SerializeField] private float sampleWindow = 0.45f;  // seconds to keep deltas
    [SerializeField] private int   maxSamples = 60;

    [Header("Detection")]
    [SerializeField] private float minTotalRadians = 3.5f; // ~200° of rotation
    [SerializeField] private float minMeanSpeed   = 400f;  // px/sec average
    [SerializeField] private float directionConsistency = 0.7f; // 0..1

    [Header("Power Mapping")]
    [SerializeField] private float minPower = 6f;
    [SerializeField] private float maxPower = 25f;

    struct Sample { public Vector2 delta; public float dt; }
    private readonly Queue<Sample> _samples = new();

    private bool   _gesturing;
    private Vector2 _lastPos;

    public bool IsGesturing => _gesturing;

    public void Begin()
    {
        _samples.Clear();
        _gesturing = true;
        _lastPos = Input.mousePosition;
    }

    public void End()
    {
        _samples.Clear();
        _gesturing = false;
    }

    void Update()
    {
        if (!_gesturing) return;

        Vector2 pos = Input.mousePosition;
        Vector2 d = pos - _lastPos;
        _lastPos = pos;

        _samples.Enqueue(new Sample { delta = d, dt = Time.unscaledDeltaTime });
        while (_samples.Count > maxSamples)
            _samples.Dequeue();
    }

    public bool TryComputeCast(out float power)
    {
        power = 0f;
        if (_samples.Count < 8) return false;

        // accumulate total signed angle from delta->delta
        float totalAngle = 0f;
        float sameSign = 0f, totalSteps = 0f;
        float dist = 0f, time = 0f;

        Vector2? prev = null;
        foreach (var s in _samples)
        {
            time += s.dt;
            dist += s.delta.magnitude;
            if (prev.HasValue)
            {
                Vector2 a = prev.Value.normalized;
                Vector2 b = s.delta.normalized;
                float ang = Mathf.Atan2(a.x*b.y - a.y*b.x, Vector2.Dot(a,b)); // signed
                totalAngle += ang;

                float sign = Mathf.Sign(ang);
                if (Mathf.Abs(ang) > 0.02f) { sameSign += sign; totalSteps++; }
            }
            prev = s.delta;
        }
        if (time <= 0f) return false;

        float meanSpeed = dist / time; // px/sec
        float consistency = totalSteps > 0 ? Mathf.Abs(sameSign)/totalSteps : 0f;

        if (Mathf.Abs(totalAngle) < minTotalRadians) return false;
        if (meanSpeed < minMeanSpeed) return false;
        if (consistency < directionConsistency) return false;

        // map |totalAngle| * meanSpeed into 0..1
        float score = Mathf.Clamp01(Mathf.Abs(totalAngle) / (2*Mathf.PI)); // 1 = ~full circle
        score *= Mathf.Clamp01(meanSpeed / (minMeanSpeed * 1.75f));

        power = Mathf.Lerp(minPower, maxPower, score);
        return true;
    }
}

}