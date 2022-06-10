using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class ShipCrewCommandOld : MonoBehaviour
{
    /*
     *This script is in charge of giving the commands of the user or AI to the crew members on board.


        private NPCInterface[] npcInterfaces;
        private ShipAmunitionInterface shipAmunitionInterface;
        private bool isFiring = false;
        private bool isReloading = false;

        private bool settingCannons = false;
        private readonly HashSet<int> cannonGroups = new HashSet<int>(); 

        // Start is called before the first frame update
        void Start()
        {
            npcInterfaces = gameObject.GetComponentsInChildren<NPCInterface>();
            shipAmunitionInterface = gameObject.GetComponent<ShipAmunitionInterface>();

        }


        public void FireCommand() {
            if (!isFiring) {

                isFiring = true;
                isReloading = false;
                FireCannons();

            }
            else {
                isFiring = false;
            }
        }

        //starts the seuqnce of choosing what cannon groups to fire.
        public void FireCommandSequence() {
            if (!settingCannons) {
                settingCannons = true;
                cannonGroups.Clear();
            }
            else {
                settingCannons = false;
                FireCommand();
            }
        }
        //api for fire command sequence
         public bool GetSettingCannonGroupStatus() {
            return settingCannons;
        }
        //reload cannons on board
        public void ReloadCommand() {
            if (!isReloading) {
                isReloading = true;
                isFiring = false;
                ReloadCannons();
            }
            else {
                isReloading = false;
            }
        }

        //This script fires the cannons on the ship. it does so by calling the fire Enum on every NPC
        private void FireCannons() {
            if (npcInterfaces == null) {
                Debug.Log(gameObject.name + "no players on ship");
            }
            else {
                //Debug.Log(gameObject.name + "start firing all cannons");
                shipAmunitionInterface.SetCannonsToNotBusy();
                foreach (NPCInterface npc in npcInterfaces) {
                    StartCoroutine(FireIEnum(npc));
                }
            }

        }

       //deals with one NPC and has it look for a new cannon after the first one is found.
        IEnumerator FireIEnum(NPCInterface npc) {

            //Debug.Log(" start coroutine loop fire: ");

            npc.SetFireCommand(true);

            while (0 != shipAmunitionInterface.GetLoadedCannons(cannonGroups).Length && isFiring) {
                    yield return StartCoroutine(npc.MoveToCannonFireIEnum(shipAmunitionInterface.GetLoadedCannons(cannonGroups)));
                    yield return new WaitForSeconds(1f);
            }

            npc.SetFireCommand(false);

            //Debug.Log(" end coroutine loop fire: ");

            isFiring = false;
        }

        //This script reloadscannons on the ship. it does so by calling the fire Enum on every NPC
        private void ReloadCannons()
        {
            if (npcInterfaces == null) {
                Debug.Log(gameObject.name + "no AI on ship");
            }
            else {
                Debug.Log(gameObject.name + "start reloading");
                shipAmunitionInterface.SetCannonsToNotBusy();
                foreach (NPCInterface npc in npcInterfaces) {
                    StartCoroutine(ReloadIEnum(npc));
                }
            }

        }


        //deals with one NPC. NPCS move from reload piles to cannons. Does this until all cannons are filled
        IEnumerator ReloadIEnum(NPCInterface npc)
        {
           // Debug.Log("start coroutine loop reload: " + npc.name);

            npc.SetReloadCommand(true);


            while (0 != shipAmunitionInterface.GetUnloadedCannons().Length && isReloading) {

                if (!npc.GetCannonBallInHand()) {
                    yield return StartCoroutine(npc.MoveToCannonBallSetIEnum(shipAmunitionInterface.GetCannonBallPiles()));

                }
                if (!isReloading) {
                    break;
                }
                yield return StartCoroutine(npc.MoveToCannonReloadIEnum(shipAmunitionInterface.GetUnloadedCannons()));
                yield return new WaitForSeconds(2);
            }

            npc.SetReloadCommand(false);

            //Debug.Log("end coroutine loop reload" + npc.name);

            isReloading = false;

        }

        public void SetCannonSets() {
            //Debug.Log("looking for input");
            if (Input.GetKeyDown("1")) {
                cannonGroups.Add(1);
                Debug.Log(gameObject.name + "added group 1");
            }
            if (Input.GetKeyDown("2")) {
                cannonGroups.Add(2);
                Debug.Log(gameObject.name + "added group 2");
            }
            if (Input.GetKeyDown("3")) {
                cannonGroups.Add(3);
                Debug.Log(gameObject.name + "added group 3");
            }
            if (Input.GetKeyDown("4")) {
                cannonGroups.Add(4);
                Debug.Log(gameObject.name + "added group 4");
            }
        }
        public void SetCannonSets(int cannonSet)
        {
            Debug.Log(gameObject.name + ": ai set group " + cannonSet);
            cannonGroups.Clear();
            cannonGroups.Add(cannonSet);
        }
        */
}
