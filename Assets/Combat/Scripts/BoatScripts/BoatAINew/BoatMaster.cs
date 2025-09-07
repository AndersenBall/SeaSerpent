using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class BoatMaster : MonoBehaviour
{
    public int tileSize = 25;
    private BoatTeamManager[] boatTeamManagers;
    
  
    List<BoatAI> allBoatsList;
    
    
    void Start()
    {
        boatTeamManagers = gameObject.GetComponentsInChildren<BoatTeamManager>();
        
        SpawnBoats();
        allBoatsList = new List<BoatAI>(gameObject.GetComponentsInChildren<BoatAI>());
        Debug.Log("Log:BoatMaster:total boats count:" + allBoatsList.Count());

    }
  

    private void SpawnBoats()
    {
        
        if (SceneTransfer.playerFleet?.GetBoats() != null) {
            var teamOneBoatTeam = boatTeamManagers.FirstOrDefault(boatTeam => boatTeam.GetTeam() == 1);
            var boats = SceneTransfer.playerFleet.GetBoats();
            int flagshipIndex = boats.FindIndex(boat => boat.boatName == SceneTransfer.playerFleet.FlagShip);

            for (int i = 0; i < boats.Count; i++) {
                if (i == flagshipIndex || (flagshipIndex == -1 && i == 0)) {
                    // Spawn the flagship or, if no flagship is found, the first boat as player-controlled
                    teamOneBoatTeam.SpawnPlayerBoat(boats[i]);
                } else {
                    // Spawn all other boats
                    teamOneBoatTeam.SpawnBoat(boats[i]);
                }
                Debug.Log("team:" + teamOneBoatTeam.GetTeam());
            }
        }
        
        if (SceneTransfer.enemyFleet != null) {
            foreach (Boat b in SceneTransfer.enemyFleet.GetBoats()) {
                foreach (BoatTeamManager boatTeam in boatTeamManagers) {
                    if (2 == boatTeam.GetTeam()) {
                        Debug.Log("team:" + boatTeam.GetTeam());
                        boatTeam.SpawnBoat(b);
                    }
                }
            }
        }
        
    }
    //adds all boats into array and sets their previous position
 


    //returns all boats on a team
    public BoatAI[] GetTeamBoats(int teamNumber)
    {
        //if team is team 1 and then object in boat manager is the correct team. return list get boats

        foreach (BoatTeamManager boatTeam in boatTeamManagers) {
            if (teamNumber == boatTeam.GetTeam()) {
                return boatTeam.GetTeamBoatAI();
            }
        }
        return null;
    }

    //gets closest boat to the vector 3 passed in
    public BoatAI GetClosestBoat(Vector3 position, int teamNum)
    {
        float distance;
        float shortestDistance = 100000000;

        BoatAI[] teamBoats = GetTeamBoats(teamNum);

        BoatAI closestBoat = null;
        if (teamBoats != null && teamBoats.Length != 0) {
            foreach (BoatAI boatCont in teamBoats) {
                if (!boatCont.isDead) {
                    
                    distance = Mathf.Pow((position.x - boatCont.transform.position.x),2) + Mathf.Pow(position.z - boatCont.transform.position.z,2);
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
    
    public void DestroyBoat(BoatAI boat) {
        if (boat.GetTeamNumber() == 1) {
            Debug.Log("Deleted:" +SceneTransfer.playerFleet.commander + boat.name);
            SceneTransfer.playerFleet.RemoveBoat(boat.name);
        }
        else {
            Debug.Log("Destroyed:" +SceneTransfer.enemyFleet.commander + boat.name);
            SceneTransfer.enemyFleet.RemoveBoat(boat.name);
        }

        Debug.Log("boat removed from boat list?:" + allBoatsList.Remove(boat));
        if (GetTeamBoats(1).Length == 0 || GetTeamBoats(2).Length == 0) {
            EndBattle();
            SceneTransfer.TransferToMap();
        }
        
        
    }

    public void EndBattle() {
        BoatAI[] allyBoatsAI = GetTeamBoats(1);
        BoatAI[] enemyBoatsAI = GetTeamBoats(1);
        List<Boat> allyBoatsData = SceneTransfer.playerFleet.GetBoats();
        List<Boat> enemyBoatsData = SceneTransfer.enemyFleet.GetBoats();

        var boatsToRemove = allyBoatsData
            .Where(boatData => !allyBoatsAI.Any(boatAI => boatAI.name == boatData.boatName))
            .ToList();
        foreach (Boat boat in boatsToRemove) {
            allyBoatsData.Remove(boat);
        }
        foreach (BoatAI boatAI in allyBoatsAI)
        {
            foreach (Boat boatData in allyBoatsData)
            {
                if (boatAI.name == boatData.boatName)
                {
                    boatData.currentBoatHealth = boatAI.GetHP();
                    Debug.Log("Boat:" + boatData.boatName + " hp:" + boatAI.GetHP());
                }
            }
        }
        SceneTransfer.playerFleet.SetBoats(allyBoatsData);
        
        var boatsToRemoveEnemy = enemyBoatsData
            .Where(boatData => !enemyBoatsAI.Any(boatAI => boatAI.name == boatData.boatName))
            .ToList();
        foreach (Boat boat in boatsToRemoveEnemy) {
            enemyBoatsData.Remove(boat);
        }
        foreach (BoatAI boatAI in enemyBoatsAI) {
            foreach (Boat boatData in enemyBoatsData) {
                if (boatAI.name == boatData.boatName) {
                    boatData.currentBoatHealth = boatAI.GetHP();
                }
            }
        }
        SceneTransfer.enemyFleet.SetBoats(enemyBoatsData);
        GameEvents.SaveGame();
    }
    
}
