using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//testing how array and list work
public class BoatMaster : MonoBehaviour
{
    public int tileSize = 25;
    private BoatTeamManager[] boatTeamManagers;
    
    List<BoatAI>[,] Arr2d = new List<BoatAI>[1000,1000];
    List<BoatAI> allBoatsList;
    
    
    void Start()
    {
        boatTeamManagers = gameObject.GetComponentsInChildren<BoatTeamManager>();

        Debug.Log("Log:BoatMaster:arr length:"+ Arr2d.Length);
        Debug.Log("Log:BoatMaster:arr .getupperbound 0:" + Arr2d.GetUpperBound(0));
        Debug.Log("Log:BoatMaster:arr .getupperbound 1:" + Arr2d.GetUpperBound(1));

        
        for (int x = 0; x <= Arr2d.GetUpperBound(0); x++) {
            for (int y = 0; y <= Arr2d.GetUpperBound(1); y++) {
                Arr2d[x, y]= new List<BoatAI>();    
            }
        }
        SpawnBoats();
        allBoatsList = new List<BoatAI>(gameObject.GetComponentsInChildren<BoatAI>());
        Debug.Log("Log:BoatMaster:total boats count:" + allBoatsList.Count());
        InitalizeBoats();

    }
    private void Update()
    {
        UpdateAll();
    }

    //updates the position of all boats checking if they moved
    private void UpdateAll() {
        foreach (BoatAI boat in allBoatsList) {
            
            Vector3 pos = boat.transform.position;
            (int x, int y) xyPos = (Mathf.FloorToInt(pos.x / tileSize), Mathf.FloorToInt(pos.z / tileSize));
            //Debug.Log("add boat: (" + boat.name + ") at location: " + xyPos.x +","+ xyPos.y);

            if (Arr2d[xyPos.x, xyPos.y].Contains(boat) == false) {
                if (Arr2d[boat.GetPrevXYPos().x, boat.GetPrevXYPos().y].Remove(boat)) { 
                    //Debug.Log("removed"); 
                }else { Debug.Log("didnt remove"); }
                //Debug.Log("boats before:" + Arr2d[boat.GetPrevXYPos().x, boat.GetPrevXYPos().y].Count+" at: " + boat.GetPrevXYPos().x+","+ boat.GetPrevXYPos().y);
                boat.SetPrevXYPos((xyPos.x, xyPos.y));
                Arr2d[xyPos.x, xyPos.y].Add(boat);
                //Debug.Log("Update: add boat: (" + boat.name + ") at location: " + xyPos.x + "," + xyPos.y);
               
                
            }
        }
    
    }

    private void SpawnBoats()
    {
        if (SceneTransfer.playerFleet != null) {
            foreach (Boat b in SceneTransfer.playerFleet.GetBoats()) {
                foreach (BoatTeamManager boatTeam in boatTeamManagers) {
                    if (1 == boatTeam.GetTeam()) {
                        Debug.Log("team:" + boatTeam.GetTeam());
                        boatTeam.SpawnBoat(b);
                    }
                }
            }
        }
        foreach (BoatTeamManager boatTeam in boatTeamManagers) {
            if (1 == boatTeam.GetTeam()) {
                boatTeam.SpawnPlayerBoat(PlayerGlobal.playerBoat);
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
    private void InitalizeBoats()
    {
        foreach (BoatAI boat in allBoatsList) {
            Vector3 pos = boat.transform.position;
            (int x, int y) xyPos = (Mathf.FloorToInt(pos.x / tileSize), Mathf.FloorToInt(pos.z / tileSize));
            Debug.Log("Log:BoatMaster:add boat: (" + boat.name + ") at location: " + xyPos.x +","+ xyPos.y);
            boat.SetPrevXYPos((xyPos.x, xyPos.y));
            Arr2d[xyPos.x, xyPos.y].Add(boat);
        }
    }

    public List<BoatAI> NearbyBoats(BoatAI boat,int range) {
        //Debug.Log("starting search for boats near: " + boat.name + " size:" + range);
        List<BoatAI> nearbyBoatsList = new List<BoatAI>();
        (int x, int y) XYPos = boat.GetPrevXYPos();
       
        if (XYPos.x >= 0 && XYPos.y >= 0 && XYPos.x < Arr2d.GetUpperBound(0) && XYPos.y < Arr2d.GetUpperBound(1)) nearbyBoatsList.AddRange(Arr2d[XYPos.x, XYPos.y]);
        nearbyBoatsList.Remove(boat);
        for (int x = 1; x <= range; x++) {
            XYPos.x--;XYPos.y++;
            
            if (XYPos.x >= 0 && XYPos.y >= 0 && XYPos.x < Arr2d.GetUpperBound(0) && XYPos.y < Arr2d.GetUpperBound(1)) nearbyBoatsList.AddRange(Arr2d[XYPos.x, XYPos.y]); 
            for (int y = 0; y < x * 2; y++) {
                XYPos.x++;
                
                if (XYPos.x >= 0 && XYPos.y >= 0 && XYPos.x < Arr2d.GetUpperBound(0) && XYPos.y < Arr2d.GetUpperBound(1)) nearbyBoatsList.AddRange(Arr2d[XYPos.x, XYPos.y]);
            }
            for (int y = 0; y < x * 2; y++) {
                XYPos.y--;
               
                if (XYPos.x >= 0 && XYPos.y >= 0 && XYPos.x < Arr2d.GetUpperBound(0) && XYPos.y < Arr2d.GetUpperBound(1)) nearbyBoatsList.AddRange(Arr2d[XYPos.x, XYPos.y]);
            }
            for (int y = 0; y < x * 2; y++) {
                XYPos.x--;
              
                if (XYPos.x >= 0 && XYPos.y >= 0 && XYPos.x < Arr2d.GetUpperBound(0) && XYPos.y < Arr2d.GetUpperBound(1)) nearbyBoatsList.AddRange(Arr2d[XYPos.x, XYPos.y]);
            }
            for (int y = 0; y < (x * 2)-1; y++) {
                XYPos.y++;
                
                if (XYPos.x >= 0 && XYPos.y >= 0 && XYPos.x < Arr2d.GetUpperBound(0) && XYPos.y < Arr2d.GetUpperBound(1)) nearbyBoatsList.AddRange(Arr2d[XYPos.x, XYPos.y]);
            }
            XYPos.y++;
        }
        
        List<BoatAI> noDup = nearbyBoatsList.Distinct().ToList();
        List<BoatAI> boatsCopy = new List<BoatAI>(noDup);
        foreach (BoatAI b in nearbyBoatsList) {
            //Debug.Log("scan near"+ boat.name+ "Boats nearby:" + b.name);
        }
        foreach (BoatAI b in boatsCopy) {
            if (b.isDead) {
                Debug.Log("missed a destroyed boat" + b.GetPrevXYPos()) ;
                noDup.Remove(b);
            }
        }
        return noDup;
    }

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

    //returns the XYCord of a spot on the map
    public (int x, int y) XYCordinates(float xCord, float yCord) {
        (int x, int y) xyPos = (Mathf.FloorToInt(xCord / tileSize), Mathf.FloorToInt(yCord / tileSize));
        return xyPos;
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

        Debug.Log("Removed from array?:" +Arr2d[boat.GetPrevXYPos().x, boat.GetPrevXYPos().y].Remove(boat));
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

        foreach (BoatAI boatAlive in allyBoatsAI) {
            foreach (Boat boatData in allyBoatsData) {
                if (boatAlive.name == boatData.boatName) {
                    boatData.SetBoatHealth(boatAlive.GetHP());
                    Debug.Log("Boat:" + boatData.boatName + " hp:" + boatAlive.GetHP());
                }
            }
        }
        SceneTransfer.playerFleet.SetBoats(allyBoatsData);
        foreach (BoatAI boatAlive in enemyBoatsAI) {
            foreach (Boat boatData in enemyBoatsData) {
                if (boatAlive.name == boatData.boatName) {
                    boatData.SetBoatHealth(boatAlive.GetHP());
                }
                
            }
        }
        SceneTransfer.enemyFleet.SetBoats(enemyBoatsData);
    }
    
}
