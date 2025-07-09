using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 *This script is in charge of giving the commands of the user or AI to the crew members on board.

*/
public class ShipCrewCommand : MonoBehaviour
{
    
    private PandaUnitAI [] unitAIs;
    private ShipAmunitionInterface shipAmunitionInterface;

    private readonly HashSet<int> cannonGroups = new HashSet<int>(); 

    // Start is called before the first frame update
    void Start()
    {
        unitAIs = gameObject.GetComponentsInChildren<PandaUnitAI>();
        shipAmunitionInterface = gameObject.GetComponent<ShipAmunitionInterface>();
        
    }

   
    //This script fires the cannons on the ship. it does so by calling the fire Enum on every NPC
    public void FireCannons() {
        if (unitAIs == null) {
            Debug.Log(gameObject.name + "no players on ship");
        }
        else {
            //Debug.Log(gameObject.name + "start firing all cannons");
            foreach (PandaUnitAI unit in unitAIs) {
                unit.cannonGroups = new HashSet<int>(cannonGroups);
                unit.SetActionNoUn("FireCannons");
            }
        }
        ClearCannons();
        
    }

    //This script fires the cannons on the ship. it does so by calling the fire Enum on every NPC
    public void FireAtWill()
    {
        if (unitAIs == null)
        {
            Debug.Log(gameObject.name + "no players on ship");
        }
        else
        {
            //Debug.Log(gameObject.name + "start firing all cannons");
            foreach (PandaUnitAI unit in unitAIs)
            {
                unit.SetActionNoUn("Gunner");
                //Debug.Log("set to gunner");
            }
            
        }
        

    }

    public void SetCannonGroups() {
        if (unitAIs == null)
        {
            Debug.Log(gameObject.name + "no players on ship");
        }
        else
        {
            foreach (PandaUnitAI unit in unitAIs)
            {
                unit.cannonGroups = new HashSet<int>(cannonGroups);
            }

        }

    }


    //This script reloads cannons on the ship. it does so by calling the fire Enum on every NPC
    public void ReloadCannons()
    {
        if (unitAIs == null) {
            Debug.Log(gameObject.name + "no players on ship");
        }
        else {
            //Debug.Log(gameObject.name + "start firing all cannons");
            foreach (PandaUnitAI unit in unitAIs) {
                unit.SetActionNoUn("ReloadCannons");
            }
        }

    }

    public void AdjustCannonAnglesPlayer() {
        
        foreach (PandaUnitAI unit in unitAIs) {
            unit.SetAction("RotateCannons");
        }
    }
 

    public void AdjustCannonAngles()
    {
        foreach (PandaUnitAI unit in unitAIs){
            unit.SetActionNoUn("RotateCannons");
        }
        ClearCannons();
    }
    public void AdjustCannonAnglePredictions(float ang) {//adjust the current amount, used by player
        CannonInterface[] cannons = shipAmunitionInterface.GetCannons(cannonGroups);
        foreach (CannonInterface cannon in cannons) {
            cannon.WantedVerticalAngle += ang;
        }
        foreach (PandaUnitAI unit in unitAIs){
            unit.SetActionNoUn("RotateCannons");
        }
    }
    public void SetCannonAnglePredictions(float x, float ang)
    {//adjust the current amount, used by player
        //Debug.Log("Log:ShipCrew:Angle:"+ang);
        CannonInterface[] cannons = shipAmunitionInterface.GetCannons(cannonGroups);
        foreach (CannonInterface cannon in cannons) {
            cannon.WantedVerticalAngle = ang;
            cannon.WantedHorizontalAngle = x;
        }
    }
    private void ActivatePredictionLines()
    {
        CannonInterface[] cannons = shipAmunitionInterface.GetCannons(cannonGroups);
        foreach (CannonInterface cannon in cannons) {
            cannon.SetLineActivity(true);
        }
    }
    private void DeactivatePredictionLines(int i)
    {
        CannonInterface[] cannons = shipAmunitionInterface.GetCannons(new HashSet<int>() { i });
        foreach (CannonInterface cannon in cannons) {
            cannon.SetLineActivity(false);
        }
    }

    public void DeactivatePredictionLines() {
        CannonInterface[] cannons = shipAmunitionInterface.GetCannons();
        foreach (CannonInterface cannon in cannons) {
            cannon.SetLineActivity(false);
        }
    }
   
    public void ClearCannons() {
        cannonGroups.Clear();
    }

    public void SetCannonSets() {
        //Debug.Log("looking for input");
        if (Input.GetKeyDown("1")) {
            if (cannonGroups.Remove(1)) {
                DeactivatePredictionLines(1);
                Debug.Log("Log:ShipCrewCommand:" + gameObject.name + "removed group 1");}
            else { 
                cannonGroups.Add(1); 
                Debug.Log("Log:ShipCrewCommand:" + gameObject.name + "added group 1");
                ActivatePredictionLines();
            }
            SetCannonGroups();
        }
        if (Input.GetKeyDown("2")) {
            if (cannonGroups.Remove(2)) {
                DeactivatePredictionLines(2);
                Debug.Log("Log:ShipCrewCommand:" + gameObject.name + "removed group 2"); }
            else {
                cannonGroups.Add(2);
                Debug.Log("Log:ShipCrewCommand:" + gameObject.name + "added group 2");
                ActivatePredictionLines();
            }
            SetCannonGroups();
        }
        if (Input.GetKeyDown("3")) {
            if (cannonGroups.Remove(3)) {
                DeactivatePredictionLines(3);
                Debug.Log("Log:ShipCrewCommand:" + gameObject.name + "removed group 3"); }
            else {
                cannonGroups.Add(3);
                Debug.Log("Log:ShipCrewCommand:" + gameObject.name + "added group 3");
                ActivatePredictionLines();
            }
            SetCannonGroups();
        }
        if (Input.GetKeyDown("4")) {
            if (cannonGroups.Remove(4)) {
                DeactivatePredictionLines(4);
                Debug.Log("Log:ShipCrewCommand:" + gameObject.name + "removed group 4"); }
            else {
                cannonGroups.Add(4);
                Debug.Log("Log:ShipCrewCommand:" + gameObject.name + "added group 4");
                ActivatePredictionLines();
            }
            SetCannonGroups();
        }
        //Debug.Log("current cannon groups:"+ cannonGroups.Count);
    }
    public void SetCannonSets(int cannonSet){
        //Debug.Log(gameObject.name + ": ai set group " + cannonSet);
        cannonGroups.Clear();
        cannonGroups.Add(cannonSet);
        SetCannonGroups();
    }
}
