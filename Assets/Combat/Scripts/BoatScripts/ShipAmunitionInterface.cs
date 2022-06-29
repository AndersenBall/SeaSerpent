//This script is used to get a over view of all cannons, guns, ammo, and interactables on the ship

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShipAmunitionInterface : MonoBehaviour
{
    CannonInterface[] cannonInterfaceList;

    public CannonInterface[] GetCannons()
    {
        cannonInterfaceList = gameObject.GetComponentsInChildren<CannonInterface>();
         return cannonInterfaceList;
    }
    public CannonInterface[] GetCannons(HashSet<int> cannonGroups)
    {
        List<CannonInterface> cannonsToReturn = new List<CannonInterface>();
        cannonInterfaceList = gameObject.GetComponentsInChildren<CannonInterface>();
        foreach (CannonInterface cannon in cannonInterfaceList) {
            if (cannonGroups.Contains(cannon.getCannonSetNum())){
                cannonsToReturn.Add(cannon);
            }
        }
        return cannonsToReturn.ToArray();
    }
    


    public CannonInterface[] GetUnloadedCannons()
    {
        cannonInterfaceList = gameObject.GetComponentsInChildren<CannonInterface>();
        //find out how many unloaded cannons there are
        int cannonCount = 0;
        int x = 0;
        foreach (CannonInterface cannon in cannonInterfaceList) {
            if (!cannon.GetLoadStatus()) {
                cannonCount++;
            }
        }

        //add cannons to list
        CannonInterface[] unloadedCannons = new CannonInterface[cannonCount];
        foreach (CannonInterface cannon in cannonInterfaceList) {
            if (!cannon.GetLoadStatus()) {
                unloadedCannons[x] = cannon;
                x++;
            }
        }

        return unloadedCannons;
    }

    public CannonInterface[] GetLoadedCannons(HashSet<int> cannonGroups)
    {
        cannonInterfaceList = gameObject.GetComponentsInChildren<CannonInterface>();
        //find out how many unloaded cannons there are
        int cannonCount = 0;
        int x = 0;
        foreach (CannonInterface cannon in cannonInterfaceList) {
            if (cannon.GetLoadStatus() && cannonGroups.Contains(cannon.getCannonSetNum())) {
                cannonCount++;
            }
        }

        //add cannons to list
        CannonInterface[] loadedCannons = new CannonInterface[cannonCount];
        foreach (CannonInterface cannon in cannonInterfaceList) {
            if (cannon.GetLoadStatus() && cannonGroups.Contains(cannon.getCannonSetNum())) {
                //Debug.Log("Loaded cannon: " + cannon.name);
                loadedCannons[x] = cannon;
                x++;
            }
        }
        
        return loadedCannons;
    }
    public CannonInterface[] GetRotateCannons() {
        List<CannonInterface> cannonsToReturn = new List<CannonInterface>();
        cannonInterfaceList = gameObject.GetComponentsInChildren<CannonInterface>();
        foreach (CannonInterface cannon in cannonInterfaceList) {
            if (cannon.GetNeedsRotation()) {
                cannonsToReturn.Add(cannon);
            }
        }
        return cannonsToReturn.ToArray();
    }
    public CannonBallSetType[] GetCannonBallPiles()
    {
        CannonBallSetType[] cannonBallTypeList = gameObject.GetComponentsInChildren<CannonBallSetType>();

        return cannonBallTypeList;
    }
}
