using ECM.Components;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    public CharacterPositionInterface positionInterface;
    public BoatControls boatControls;
    public MouseLookBoat mouseLookBoat;
    public MapCamera mapCamera;
    public RTSBoxSelection unitSelection;
    private PlayerControlMode playerMode;
    private BoatControlMode boatMode;
    private MapControlMode mapMode;
    private ControlModeManager modeManager;
    private HUDController hudController;
    private MouseLook mouseLook;
    private PlayerTriggerController triggerController;

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
        triggerController = ecmController.GetComponent<PlayerTriggerController>();

        // Find Hud Control
        GameObject canvas = GameObject.Find("Canvas");
        hudController = canvas.GetComponent<HUDController>();


        boatMode = new BoatControlMode(positionInterface, boatControls, hudController,mouseLookBoat);
        playerMode = new PlayerControlMode(triggerController,positionInterface, hudController, mouseLook);
        mapMode = new MapControlMode(hudController, mapCamera, unitSelection);
        modeManager.SetMode(playerMode);
    }

    private void Update()
    {
        // Swap to Boat Control Mode 
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (modeManager.CurrentMode == boatMode)
            {
                modeManager.SetMode(playerMode);
            }
            else
            {
                modeManager.SetMode(boatMode);
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (modeManager.CurrentMode == mapMode)
            {
                modeManager.SetMode(playerMode); 
            }
            else
            {
                modeManager.SetMode(mapMode);
            }
        }
    }
}
