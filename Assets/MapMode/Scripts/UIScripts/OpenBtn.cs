using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenBtn : MonoBehaviour
{
    public void openAndClose()
    {
        if (gameObject.active == true)
        {
            gameObject.active = false;
        }
        else if (gameObject.active == false)
        {
            gameObject.active = true;
        }
    }
}
