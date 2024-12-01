using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModeManager : MonoBehaviour
{
    private ControlMode currentMode;

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


    private void Update()
    {
        if (currentMode != null)
        {
            currentMode.UpdateMode();
        }
    }
}

