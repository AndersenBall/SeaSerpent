using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlMode
{
    public abstract void EnterMode();
    public abstract void UpdateMode();
    public abstract void ExitMode();
}
