using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public GameObject messagePanel;
    public GameObject cannonBallIcon;
    public GameObject cannonGroups;
    public GameObject sailStengthText;
    public GameObject healthbar;
    public Camera firstPersonCamera;
    public Camera overheadCamera;
    public Camera mapCamera;



    // Call this function to disable FPS camera,
    // and enable overhead camera.
    public void UpdateSailStength((float ai, float cur)speed) {
        if (speed.ai != null && speed.cur != null) {
            sailStengthText.GetComponent<UnityEngine.UI.Text>().text = "Sail Stength " + speed.ai + "\nCurrent Speed " + speed.cur;
        }
    }

    public void UpdateHealth(float hp) {
        Debug.Log("Info:HUD:update player hp"+hp);
        healthbar.GetComponent<UnityEngine.UI.Text>().text = "Health " + hp;
    }

    public void ShowOverheadView()
    {
        firstPersonCamera.enabled = false;
        mapCamera.enabled = false;
        overheadCamera.enabled = true;

    }

    public void ShowMapView()
    {
        firstPersonCamera.enabled = false;
        overheadCamera.enabled = false;
        mapCamera.enabled = true;
    }

    // Call this function to enable FPS camera,
    // and disable overhead camera.
    public void ShowFirstPersonView()
    {
        mapCamera.enabled = false;
        overheadCamera.enabled = false;
        firstPersonCamera.enabled = true;
    }
    public void CannonGroupHelpOn() {
        cannonGroups.active = true;
    }
    public void CannonGroupHelpOff() {
        cannonGroups.active = false;
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
