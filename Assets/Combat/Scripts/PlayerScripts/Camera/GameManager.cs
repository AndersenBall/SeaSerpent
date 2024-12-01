//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GameManager : MonoBehaviour
//{
//    public ControlModeManager modeManager;

//    //private BoatControlMode boatMode = new BoatControlMode();
//    //private PlayerControlMode playerMode = new PlayerControlMode();
//    //private MapControlMode mapMode = new MapControlMode();

//    private bool isInSteeringWheelCollider = false; // Tracks if the player is inside the steering wheel collider

//    private void Start()
//    {
//        // Start the game in player control mode
//        modeManager.SetMode(playerMode);
//    }

//    void Update()
//    {
//        // Toggle map mode when M is pressed
//        if (Input.GetKeyDown(KeyCode.M))
//        {
//            modeManager.ToggleMapMode(mapMode, playerMode);
//        }

//        // Check if the player can enter boat mode (inside collider and presses B)
//        if (isInSteeringWheelCollider && Input.GetKeyDown(KeyCode.B))
//        {
//            modeManager.SetMode(boatMode);
//        }

//        // Exit boat mode when E is pressed, always return to player mode
//        if (Input.GetKeyDown(KeyCode.E) && modeManager.CurrentMode == boatMode)
//        {
//            modeManager.ExitBoatMode(playerMode);
//        }
//    }

//    // Detect if the player enters the steering wheel's collider
//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.CompareTag("SteeringWheel")) // Assuming the steering wheel collider is tagged as "SteeringWheel"
//        {
//            isInSteeringWheelCollider = true;
//        }
//    }

//    // Detect if the player exits the steering wheel's collider
//    private void OnTriggerExit(Collider other)
//    {
//        if (other.CompareTag("SteeringWheel"))
//        {
//            isInSteeringWheelCollider = false;
//        }
//    }
//}

