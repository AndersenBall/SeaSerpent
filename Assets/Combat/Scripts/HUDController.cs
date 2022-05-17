using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public GameObject messagePanel;
    public GameObject cannonBallIcon;
    public GameObject CannonGroups;
    public GameObject SailStengthText;
    public Camera firstPersonCamera;
    public Camera overheadCamera;



    // Call this function to disable FPS camera,
    // and enable overhead camera.
    public void UpdateSailStength((float ai, float cur)speed) {
        if (speed.ai != null && speed.cur != null) {
            SailStengthText.GetComponent<UnityEngine.UI.Text>().text = "Sail Stength " + speed.ai + "\nCurrent Speed " + speed.cur;
        }
    }

    public void ShowOverheadView()
    {
        firstPersonCamera.enabled = false;
        overheadCamera.enabled = true;
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void ShowFirstPersonView()
    {
        firstPersonCamera.enabled = true;
        overheadCamera.enabled = false;
    }
    public void CannonGroupHelpOn() {
        CannonGroups.active = true;
    }
    public void CannonGroupHelpOff() {
        CannonGroups.active = false;
    }
    public void cannonKeyOn()
    {
        messagePanel.SetActive(true);
    }

    public void cannonKeyOff()
    {
        messagePanel.SetActive(false);
    }

    public void cannonIconOn()
    {
        cannonBallIcon.SetActive(true);
    }
    public void cannonIconOff()
    {
        cannonBallIcon.SetActive(false);
    }
}
