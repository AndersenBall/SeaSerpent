using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class BoatMaster : MonoBehaviour
{
    public int tileSize = 25;
    private BoatTeamManager[] boatTeamManagers;
    
    List<BoatAI>[,] Arr2d = new List<BoatAI>[1000,1000];
    List<BoatAI> allBoatsList;

    private void Awake()
    {
        LoadFleet();
    }

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
        SavePlayerFleet(SceneTransfer.playerFleet);


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
        if (SceneTransfer.playerFleet.GetBoats().Count == 0){
            SceneTransfer.TransferToTownUI();
            PlayerGlobal.money -= PlayerGlobal.money / 2;
            return;
            //TODO 8/16 move location to starting town? set transfer what town you at in the UI
        }
        SceneTransfer.TransferToMap();
    }
    
    #region save load
    public void SavePlayerFleet(Fleet fleet)
    {
        Vector3 targetPosition = new(0, 0, 0); //TODO PUT TO base cordinates of a town
        if (SaveLoad.SaveExists("Player")) {
            PlayerFleetMapController.PlayerFleetData playerData = SaveLoad.Load<PlayerFleetMapController.PlayerFleetData>("Player");
            targetPosition = new(playerData.pos[0], playerData.pos[1], playerData.pos[2]);
        }
      
        
        fleet.CalculateSpeed();
        PlayerFleetMapController.PlayerFleetData saveFleet = new PlayerFleetMapController.PlayerFleetData
        {
            fleet = fleet, 
            pos = new float[] 
            {
                targetPosition.x,
                targetPosition.y,
                targetPosition.z
            }
        };
        SaveLoad.Save(saveFleet, "Player");
        Debug.Log("Player Fleet saved successfully.");
    }
    
    
    public void LoadFleet()
    {
        if (SaveLoad.SaveExists("Player")) {
            PlayerFleetMapController.PlayerFleetData playerData = SaveLoad.Load<PlayerFleetMapController.PlayerFleetData>("Player");
            SceneTransfer.playerFleet = playerData.fleet;
        }

    }

    
    #endregion
    
}
