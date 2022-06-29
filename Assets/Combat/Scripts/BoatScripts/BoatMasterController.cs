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
 

}
