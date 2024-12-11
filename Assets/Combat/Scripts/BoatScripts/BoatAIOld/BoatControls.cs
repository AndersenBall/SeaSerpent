
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
    bool isPlayersBoat = false;
    bool isPlayerDriving = false;
    bool isDead = false;

    bool speedChanging = false;//is the speed currently being moved towards
    #endregion

    #region Monobehaviors
    void Start() {
        hudControl = GameObject.Find("HUD/Canvas").GetComponent<HUDController>();
        animator = gameObject.GetComponent<Animator>();
        boatPhysics = gameObject.GetComponent<BoatAlignNormal>();
        boatAI = gameObject.GetComponent<BoatAI>();
        
    }
    private void Update()
    {
        if (isPlayersBoat){
            (float, float) values = GetBoatSpeed();
            hudControl.UpdateSailStength((Mathf.Round(values.Item1 * 10) / 10f, Mathf.Round(values.Item2 * 10) / 10f));
        }
    }
    #endregion

    #region setters and getters
    public void SetPlayerControlled(bool player) {
        isPlayersBoat = player;
    }
    public bool GetPlayerControlled() {
        return isPlayersBoat;
    }
    /*
    public void SetBoatParamters(Boat b) {
        //Debug.Log(b.GetTurnSpeed()) ;
        boat = b;
        boatPhysics = gameObject.GetComponent<BoatAlignNormal>();//as it hasnt spawned yet
        boatPhysics.SetBoatSpeed(b.GetSpeed(), b.GetTurnSpeed());
        gameObject.GetComponentInChildren<BoatHealth>().SetHealth(b.GetBoatHealth());
    }*/
    public void SetBoatParamters(Boat b,bool playerControlled)
    {
        //Debug.Log(b.GetTurnSpeed()) ;
        SetPlayerControlled(playerControlled);
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
    #endregion

    #region methods
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
    #endregion


}
