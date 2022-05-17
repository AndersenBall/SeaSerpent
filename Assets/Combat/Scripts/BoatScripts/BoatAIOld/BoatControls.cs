
using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class BoatControls : MonoBehaviour
{
    #region variables
    private BoatAlignNormal boatPhysics;
    private Animator animator;
    private BoatAI boatAI;
    private HUDController hudControl;
    private Boat boat;

    //values used in program to control boat
    float rawForward = 0;//current forward speed
    float aiForward = 0;//goal forward speed
    float sideways = 0;
    bool isPlayerDriving = false;
    bool isDead = false;

    bool speedChanging = false;//is the speed currently being moved towards
    #endregion

    #region Monobehaviors
    void Start() {
        hudControl = gameObject.GetComponent<HUDController>();
        animator = gameObject.GetComponent<Animator>();
        boatPhysics = gameObject.GetComponent<BoatAlignNormal>();
        boatAI = gameObject.GetComponent<BoatAI>();
        
    }
    #endregion
    

    public void SetBoatParamters(Boat b) {
        //Debug.Log(b.GetTurnSpeed()) ;
        boat = b;
        boatPhysics = gameObject.GetComponent<BoatAlignNormal>();//as it hasnt spawned yet
        boatPhysics.SetBoatSpeed(b.GetSpeed(), b.GetTurnSpeed());
        gameObject.GetComponentInChildren<BoatHealth>().SetHealth(b.GetBoatHealth());
    }
    
    public (float, float) GetBoatSpeed() {//human input speeds
        return (aiForward, rawForward);
    }

    public void SetIsPlayerDriving(bool drive) {
        //animator.SetBool("ControlledByPlayer",drive);
        isPlayerDriving = drive;
    }

    //legacy and now outdated
    private void PlayerDrive() {
        if (isPlayerDriving) {
            rawForward += Input.GetAxis("Vertical") * .1f * Time.deltaTime;
            
            sideways = (Input.GetKey(KeyCode.A) ?  -1f : 0f) +
                        (Input.GetKey(KeyCode.D) ?  1f : 0f);    
        }
        else {
            if(rawForward > aiForward) {
                rawForward -= .1f * Time.deltaTime;
            }
            if (rawForward < aiForward) {
                rawForward += .1f * Time.deltaTime;
            }
        }

        boatPhysics.SetSteer(sideways);

        rawForward = Mathf.Clamp(rawForward, 0, 1);
        boatPhysics.SetThrotal(rawForward);
    }
    

    public void SetTurn(float turn) {
        turn = Mathf.Clamp(turn, -1, 1);
        boatPhysics.SetSteer(turn);
    }

    //sets the amount of throtal that the boat will work towards reaching
    public float GetForward() {
        return boatPhysics.GetThrotal();
    }
    public float GetEnginePower() {
        return boatPhysics.GetBoatEnginePower();
    }
    public void SetForward(float power) {
        if (speedChanging == false) {
            StartCoroutine(MoveToSpeed(power));
        }
        else {
            aiForward = power;
        }
        //Debug.Log("forward speed: " + aiForward + "raw forward: " + rawForward);
    }
    //moves the boat speed .01 closer every .1 seconds until the input speed is reached. 
    IEnumerator MoveToSpeed(float speed) {
        speedChanging = true;
        aiForward = speed;
        while (aiForward != rawForward) {
            if (rawForward > aiForward) {
                rawForward -= .01f;
            }else if (rawForward < aiForward) {
                rawForward += .01f;
            }
            rawForward = Mathf.Clamp(rawForward, 0, 1);
            
            boatPhysics.SetThrotal(rawForward);
            yield return new WaitForSeconds(.1f);
        }
        speedChanging = false;
    }

    public void Die() {
        isDead = true;
        boatAI.SetIsDead(true);
        boatPhysics._buoyancyCoeff = 0;

    }

    public bool GetIsDead() {
        return isDead;
    }


 
}
