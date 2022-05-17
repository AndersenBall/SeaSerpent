using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NPCInterface : MonoBehaviour
{
    private ShipAmunitionInterface shipAmunitions;

    private AIMovement movement;

    private GameObject cannonBall;
    public GameObject cannonBallPrefab;

    private Collider triggerCollider;

    private bool fireCommand = false;
    private bool reloadCommand = false;
    //private bool repairCommand = false;
    private bool hasCannonBall = false;


    // Start is called before the first frame update
    void Start()
    {
        shipAmunitions = transform.parent.parent.GetComponent<ShipAmunitionInterface>();

        movement = gameObject.GetComponent<AIMovement>();
    }

    //this command sets if the NPC will trigger guns it walks into
    public bool SetFireCommand(bool fire)
    {
        fireCommand = fire;
        return fireCommand;
    }
    public bool SetReloadCommand(bool load)
    {
        reloadCommand = load;
        return reloadCommand;
    }

    public bool GetCannonBallInHand() {
        return hasCannonBall;
    }
    //finds the nearest cannon that isnt busy
    public CannonInterface FindNearestCannon(CannonInterface[] ObjectList)
    {
        CannonInterface nearestCannon = null;
        if (ObjectList != null && ObjectList.Length != 0) {
            
            float shortestDistance = 100f;
            float distance;

            foreach (CannonInterface cannon in ObjectList) {
                if (!cannon.GetBusyStatus()) {  
                    distance = Vector3.Distance(transform.position, cannon.transform.position);
                    //Debug.Log("Cannon: " + cannon.name + "Distance: " + distance);
                    if (shortestDistance > distance) {
                        shortestDistance = distance;

                        
                        nearestCannon = cannon;
                        
                        //    Debug.Log("new shortest distance: " + shortestDistance);
                    }
                }
            }
            if (nearestCannon != null) {
                nearestCannon.SetBusyStatus(true);
            }
        }
        return nearestCannon;
    }

    //this script is a coroutine that moves the player to the cannons
    public IEnumerator MoveToCannonFireIEnum(CannonInterface[] cannonControler)
    {
        //Debug.Log("start Coroutine move to fire cannon: ");

        if (cannonControler.Length == 0) {
            Debug.Log("can not find a cannon ");
            yield break;
        }

        CannonInterface cannon = FindNearestCannon(cannonControler);

        if (cannon == null) {
            yield break;
        }

        movement.SetDestination(cannon.transform.position - 3 * cannon.transform.forward);

        while (true) {
            if (Vector3.Distance(transform.position, cannon.transform.position - 3 * cannon.transform.forward) < 1f) {
                cannon.Fire();  
                break;
            }
            if (!fireCommand) {
                
                cannon.SetBusyStatus(false);
                break;
            }
            yield return null;
        }
        //Debug.Log("end Coroutine move to fire cannon: ");
    }

    public IEnumerator MoveToCannonReloadIEnum(CannonInterface[] cannonControler)
    {
        //Debug.Log("start Coroutine move to reload cannon: " + gameObject.name);

        if (cannonControler.Length == 0) {
            yield break;
        }

        CannonInterface cannon = FindNearestCannon(cannonControler);

        if (cannon == null) {
            yield break;
        }
        movement.SetDestination(cannon.transform.position - 3 * cannon.transform.forward);

        while (true) {
            if (Vector3.Distance(transform.position, cannon.transform.position - 3 * cannon.transform.forward) < 1f) {
                if (hasCannonBall) {
                    cannon.LoadGun();
                    hasCannonBall = false;
                    Destroy(cannonBall);
                }
                else {
                    cannon.SetBusyStatus(false);
                }
                break;
            }
            if (!reloadCommand) {
                cannon.SetBusyStatus(false);
                break;
            }
            yield return null;
        }
        //Debug.Log("end Coroutine move to reload cannon: " + gameObject.name);
    }

    //finds the nearest cannon Ball Set
    public CannonBallSetType FindNearestCannonBallSet(CannonBallSetType[] ObjectList)
    {

        CannonBallSetType cannonBallSetType = null;
        if (ObjectList != null && ObjectList.Length != 0) {
            cannonBallSetType = ObjectList[0];
            float shortestDistance = Vector3.Distance(transform.position, cannonBallSetType.transform.position);
            float distance;

            foreach (CannonBallSetType cannonBall in ObjectList) {

                distance = Vector3.Distance(transform.position, cannonBall.transform.position);
                //Debug.Log("Cannon: " + cannon.name + "Distance: " + distance);
                if (shortestDistance > distance) {
                    shortestDistance = distance;
                    cannonBallSetType = cannonBall;
                    //    Debug.Log("new shortest distance: " + shortestDistance);
                }

            }
        }
        return cannonBallSetType;
    }

    //this script is a coroutine that moves the player to the CannonBallSet
    public IEnumerator MoveToCannonBallSetIEnum(CannonBallSetType[] cannonBallSets)
    {
        //Debug.Log("start Coroutine move to cannonball set: " + gameObject.name);

        //CannonBallSetType[] cannonBallSets = shipAmunitions.GetCannonBallPiles();

        if (cannonBallSets != null && cannonBallSets.Length != 0) {
            CannonBallSetType cannonBallSet = FindNearestCannonBallSet(cannonBallSets);

            movement.SetDestination(cannonBallSet.transform.position);

            while (reloadCommand) {
                
                if(Vector3.Distance(transform.position, cannonBallSet.transform.position) < 1f) {

                    if (reloadCommand && !hasCannonBall) {
                        //Debug.Log("reloaded");
                        hasCannonBall = true;
                        cannonBall = Instantiate(cannonBallPrefab, gameObject.transform);
                        cannonBall.transform.position = cannonBall.transform.position + new Vector3(1, 3, 0);
                    }
                    break;
                }
                yield return null;
                //Debug.Log(Vector3.Distance(transform.position, cannonBallSet.transform.position - 3 * cannonBallSet.transform.forward));
            }
            
        }
        //Debug.Log("end Coroutine move to cannonball set: " + gameObject.name);
    }
    private void OnTriggerEnter(Collider other)
    {
        triggerCollider = other;
    }
    private void OnTriggerExit(Collider other) {
        if (other == triggerCollider) {
            
            triggerCollider = null;
        }
    }
   
}
