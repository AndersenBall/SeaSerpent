using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeManager : MonoBehaviour
{
    private ControlMode currentMode;
    private bool isMapModeActive = false;
    private bool isBoatModeActive = false;

    public ControlMode CurrentMode
    {
        get { return currentMode; }
    }

    public void SetMode(ControlMode newMode)
    {
        if (currentMode != null)
        {
            currentMode.ExitMode();
        }

        currentMode = newMode;
        currentMode.EnterMode();
    }

    // Toggle between map mode and previous mode
    public void ToggleMapMode(ControlMode mapMode, ControlMode previousMode)
    {
        if (!isMapModeActive)
        {
            // Store the previous mode and switch to the map mode
            SetMode(mapMode);
            isMapModeActive = true;
        }
        else
        {
            // Switch back to the previous mode
            SetMode(previousMode);
            isMapModeActive = false;
        }
    }

    // Exit boat mode directly to player mode
    public void ExitBoatMode(ControlMode playerMode)
    {
        if (isBoatModeActive)
        {
            SetMode(playerMode); // Always switch back to player mode
            isBoatModeActive = false;
        }
    }

    private void Update()
    {
        if (currentMode != null)
        {
            currentMode.UpdateMode();
        }
    }
}

