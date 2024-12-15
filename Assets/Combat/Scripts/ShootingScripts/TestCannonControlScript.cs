using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCannonControlScript : MonoBehaviour
{
    public CannonInterface cannonInterface; // Reference to the CannonInterface script
    public Transform enemyTransform;

    public float desiredVerticalAngle = 5f;    // Example vertical angle
    public float desiredHorizontalAngle = 10f; // Example horizontal angle
    public float predictedHorizontalAngle;

    // Update is called once per frame
    void Update()
    {
        // Check if the H key is pressed
        if (cannonInterface != null && enemyTransform != null)
        {
            // Calculate the direction and distance to the enemy
            Vector3 directionToEnemy = enemyTransform.position - cannonInterface.transform.position;
            float horizontalDistance = new Vector3(directionToEnemy.x, 0, directionToEnemy.z).magnitude; // Ignore vertical component

            // Predict the vertical angle using the horizontal distance
            float predictedVerticalAngle = PredictCannonAngle(horizontalDistance);
            Vector3 localDirection = cannonInterface.transform.InverseTransformDirection(directionToEnemy.normalized);

            float predictedHorizontalAngle = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;




            // Pass the predicted vertical angle to the cannon interface
            cannonInterface.WantedVerticalAngle = predictedVerticalAngle;
            cannonInterface.WantedHorizontalAngle = predictedHorizontalAngle;

            Debug.Log($"Aiming cannon: Vertical angle: {predictedVerticalAngle}");

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (cannonInterface.GetLoadStatus()) // Ensure cannon is loaded
                {
                    cannonInterface.Fire();
                    Debug.Log("Cannon fired!");
                    cannonInterface.LoadGun();
                    cannonInterface.SetLineActivity(true);
                }
                else
                {
                    Debug.LogWarning("Cannon is not loaded!");
                }
            }
        }
        else
        {
            Debug.LogError("CannonInterface or EnemyTransform reference is not assigned!");
        }
    }

    private float PredictCannonAngle(float distance)
    {
        float value = Mathf.Asin((distance - 7) / 4077.47f) / 2 * (180 / Mathf.PI);
        if (float.IsNaN(value))
        {
            return 45f; // Default angle if prediction fails
        }
        else
        {
            return value;
        }
    }
}
