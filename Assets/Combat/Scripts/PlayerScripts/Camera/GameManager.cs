using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    public CharacterPositionInterface positionInterface;
    public BoatControls boatControls;

    private BoatControlMode boatMode;
    private ControlModeManager modeManager;


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

        // get boat controller on unist parent
        GameObject ecmController = GameObject.Find("ECM_BaseFirstPersonController");
        boatControls = ecmController.GetComponentInParent<BoatControls>();


        boatMode = new BoatControlMode(positionInterface, boatControls);
        
    }

    private void Update()
    {
        // Swap to Boat Control Mode when near the steering wheel
        if (Input.GetKeyDown(KeyCode.E))
        {

            // TODO 12/1 can remove when modes are made on startup
            boatMode = new BoatControlMode(positionInterface, boatControls);
            modeManager.SetMode(boatMode);
        }

        // Exit Boat Control Mode to Player Control Mode
        else if (Input.GetKeyDown(KeyCode.E) && modeManager.CurrentMode == boatMode)
        {
            //modeManager.SetMode(playerMode);
        }
    }
}
