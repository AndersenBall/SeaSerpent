using UnityEngine;

public class SpeedController : MonoBehaviour
{
    // The default time scale, typically 1.0 (normal speed)
    private float defaultTimeScale = 1.0f;

    // The speed adjustment value
    public float speedStep = 0.1f;

    // Minimum and maximum limits for the time scale
    public float minTimeScale = 0.1f;
    public float maxTimeScale = 3.0f;

    void Update()
    {
        // Check for '+' key press to speed up
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) // 'Equals' is for '=' key (same as '+') on standard keyboards
        {
            AdjustTimeScale(speedStep);
        }

        // Check for '-' key press to slow down
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) // For both '-' on keyboard and numeric keypad
        {
            AdjustTimeScale(-speedStep);
        }

        // Reset speed to default if 'R' is pressed
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ResetTimeScale();
        }
    }

    // Method to adjust the time scale
    private void AdjustTimeScale(float adjustment)
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale + adjustment, minTimeScale, maxTimeScale);
        Debug.Log("Time Scale: " + Time.timeScale);
    }

    // Method to reset the time scale to default
    private void ResetTimeScale()
    {
        Time.timeScale = defaultTimeScale;
        Debug.Log("Time Scale Reset to Default: " + Time.timeScale);
    }
}