using ECM.Components;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    public CharacterPositionInterface positionInterface;
    public BoatControls boatControls;
    public MouseLookBoat mouseLookBoat;
    public MapCamera mapCamera;
    public RTSBoxSelection unitSelection;
    public CannonCameraControl cameraControl;
    private CannonControlMode cannonMode;
    private PlayerControlMode playerMode;
    private BoatControlMode boatMode;
    private MapControlMode mapMode;
    private ControlModeManager modeManager;
    private HUDController hudController;
    private MouseLook mouseLook;
    private MouseLookPlayer mouseLookPlayer;
    private PlayerTriggerController triggerController;

    private bool canToggleMode = true;

    private void Start()
    {

        modeManager = GetComponent<ControlModeManager>();
        

        // Find CharacterPositionInterface on ECM_BaseFirstPersonControllerAI
        GameObject ecmControllerAi = GameObject.Find("ECM_BaseFirstPersonControllerAI");
        positionInterface = ecmControllerAi.GetComponent<CharacterPositionInterface>();

        // get boat controller on units parent
        GameObject ecmController = GameObject.Find("ECM_BaseFirstPersonController");
        boatControls = ecmController.GetComponentInParent<BoatControls>();
        mouseLook = ecmController.GetComponent<MouseLook>();
        mouseLookPlayer = ecmController.GetComponentInChildren<MouseLookPlayer>();
        triggerController = ecmController.GetComponent<PlayerTriggerController>();

        // Find Hud Control
        GameObject canvas = GameObject.Find("Canvas");
        hudController = canvas.GetComponent<HUDController>();

        cannonMode = new CannonControlMode(hudController, triggerController, cameraControl);
        boatMode = new BoatControlMode(positionInterface, boatControls, hudController,mouseLookBoat);
        playerMode = new PlayerControlMode(triggerController,positionInterface, hudController, mouseLook,mouseLookPlayer);
        mapMode = new MapControlMode(hudController, mapCamera, unitSelection);
        modeManager.SetMode(playerMode);
    }

    private void Update()
    {
        if (!canToggleMode) return;
        if (Input.GetKeyDown(KeyCode.E) )
        {
            if (modeManager.CurrentMode == cannonMode)
            {
                modeManager.SetMode(playerMode); // Exit cannonMode to playerMode
            }
            else if (triggerController.activeCannon != null)
            {
                modeManager.SetMode(cannonMode); // Enter cannonMode
            }

            StartCoroutine(ResetToggle());
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            if (modeManager.CurrentMode == boatMode)
            {
                modeManager.SetMode(playerMode);
            }
            else
            {
                modeManager.SetMode(boatMode);
            }
            StartCoroutine(ResetToggle());
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            if (modeManager.CurrentMode == mapMode)
            {
                modeManager.SetMode(playerMode); 
            }
            else
            {
                modeManager.SetMode(mapMode);
            }
            StartCoroutine(ResetToggle());
        }
    }

    private IEnumerator ResetToggle()
    {
        canToggleMode = false;
        yield return new WaitForSeconds(0.4f); // Adjust delay as needed
        canToggleMode = true;
    }
}
