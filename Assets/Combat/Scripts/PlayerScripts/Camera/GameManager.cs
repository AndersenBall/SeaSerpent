using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    public CharacterPositionInterface positionInterface;
    public BoatControls boatControls;
    private PlayerControlMode playerMode;
    private BoatControlMode boatMode;
    private ControlModeManager modeManager;
    private HUDController hudController;


    private void Start()
    {

        modeManager = GetComponent<ControlModeManager>();
        if (modeManager == null)
        {
            Debug.LogError("ControlModeManager is missing on the same GameObject as GameManager!");
            return;
        }

        // Find CharacterPositionInterface on ECM_BaseFirstPersonControllerAI
        GameObject ecmControllerAi = GameObject.Find("ECM_BaseFirstPersonControllerAI");
        positionInterface = ecmControllerAi.GetComponent<CharacterPositionInterface>();

        // get boat controller on units parent
        GameObject ecmController = GameObject.Find("ECM_BaseFirstPersonController");
        boatControls = ecmController.GetComponentInParent<BoatControls>();

        // Find Hud Control
        GameObject canvas = GameObject.Find("Canvas");
        hudController = canvas.GetComponent<HUDController>();


        boatMode = new BoatControlMode(positionInterface, boatControls, hudController);
        playerMode = new PlayerControlMode(positionInterface, hudController);
        
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
    }
}
