using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// legacy
//this is an interface used by the boats to get information on boats over the entire world
public class BoatMasterController : MonoBehaviour
{
    private BoatTeamManager[] boatTeamManager;

    void Start()
    {
        boatTeamManager = gameObject.GetComponentsInChildren<BoatTeamManager>();
        
    }

    //returns all boats on a team
    public BoatAI[] GetTeamBoats(int teamNumber) {
        //if team is team 1 and then object in boat manager is the correct team. return list get boats
        
        foreach (BoatTeamManager boatTeam in boatTeamManager){
            if (teamNumber == boatTeam.GetTeam()) {
                return boatTeam.GetTeamBoatAI();
            }
        }
        
        return null;
    }

    //gets closest boat to the vector 3 passed in
    public BoatAI GetClosestBoat(Vector3 position, int teamNum) {
        float distance;
        float shortestDistance = 100000000;
        
        BoatAI[] teamBoats = GetTeamBoats(teamNum);

        BoatAI closestBoat = null;
        if (teamBoats != null && teamBoats.Length != 0) {
            foreach (BoatAI boatCont in teamBoats) {
                if(!boatCont.GetIsDead()){
                    distance = Vector3.Distance(position, boatCont.transform.position);
                    //Debug.Log("Cannon: " + cannon.name + "Distance: " + distance);
                    if (distance > .1f) { 
                        if (shortestDistance > distance) {
                                shortestDistance = distance;
                                closestBoat = boatCont;
                                //    Debug.Log("new shortest distance: " + shortestDistance);
                            }
                    }   
                }
            }
        }
        return closestBoat;
    }

    

}
