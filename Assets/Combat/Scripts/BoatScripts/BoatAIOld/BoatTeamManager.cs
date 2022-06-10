using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoatTeamManager : MonoBehaviour
{
    
    private BoatAI[] boatAIList;
    private int numberOfBoats = 0;
    public int TeamNumber;
    public Transform boatAiTransform;
    public GameObject boatAIPrefab;
    public GameObject unitAIPrefab;
    public GameObject boatPrefab;
    public GameObject unitPrefab;
    public GameObject flagPrefab;
    
    void Start()
    {
        boatAIList = gameObject.GetComponentsInChildren<BoatAI>();
        TeamNumber = gameObject.layer == 13 ? 1 : 2;
        
        foreach (BoatAI b in boatAIList) {
            b.SetTeamNumber(TeamNumber);
        }    
    }

    
    public BoatAI[] GetTeamBoatAI()
    {
        boatAIList = gameObject.GetComponentsInChildren<BoatAI>();
        //Debug.Log("boatAI length before:" + boatAIList.Length);
        boatAIList = boatAIList.Where(e => e.isDead != true).ToArray();
        //Debug.Log("boatAI length after:" + boatAIList.Length);
        return boatAIList;
    }
    public void SpawnBoat(Boat b) {
        int numberOfSailors = 0;
        GameObject spawnedAIBoat = Instantiate(boatAIPrefab, boatAiTransform);
        spawnedAIBoat.transform.localPosition = new Vector3(numberOfBoats*30, 0, 0);
        GameObject spawnedBoat = Instantiate(boatPrefab, this.transform);
        spawnedBoat.transform.localPosition = new Vector3(numberOfBoats*100,0,Random.Range(-100,100));
        spawnedBoat.GetComponent<BoatControls>().SetBoatParamters(b);
        spawnedBoat.GetComponent<BoatAI>().SetTeamNumber(TeamNumber);
        spawnedBoat.name = b.boatName;
        GameObject flag = Instantiate(flagPrefab, spawnedBoat.transform);
        flag.transform.position = spawnedBoat.transform.position+ new Vector3(0, 44, -10.47f);

        numberOfBoats += 1;
        foreach (Sailor s in b.GetSailors()) {
            GameObject spawnedUnit =Instantiate(unitPrefab, spawnedBoat.transform.Find("Crew"));
            GameObject spawnedAIUnit = Instantiate(unitAIPrefab, spawnedAIBoat.transform);
            spawnedUnit.name = s.name;
            spawnedAIUnit.name = "AI"+ s.name;
            spawnedUnit.GetComponent<AIMovement>().navMeshInterface = spawnedAIUnit.GetComponent<NavMeshUnitInterface>();
            spawnedAIUnit.transform.position += new Vector3(0, 0, numberOfSailors*2);
            numberOfSailors += 1;
        }
    }
    public void SpawnPlayerBoat(Boat b)
    {
        int numberOfSailors = 0;
        GameObject spawnedAIBoat = Instantiate(boatAIPrefab, boatAiTransform);
        spawnedAIBoat.transform.localPosition = new Vector3(numberOfBoats * 30, 0, 0);
        GameObject spawnedBoat = Instantiate(boatPrefab, this.transform);
        spawnedBoat.transform.localPosition = new Vector3(numberOfBoats * 100, 0, Random.Range(-100, 100));
        spawnedBoat.GetComponent<BoatControls>().SetBoatParamters(b);
        spawnedBoat.GetComponent<BoatAI>().SetTeamNumber(TeamNumber);
        spawnedBoat.name = b.boatName;
        GameObject flag = Instantiate(flagPrefab, spawnedBoat.transform);
        flag.transform.position = spawnedBoat.transform.position + new Vector3(0, 44, -10.47f);

        numberOfBoats += 1;
        foreach (Sailor s in b.GetSailors()) {
            GameObject spawnedUnit = Instantiate(unitPrefab, spawnedBoat.transform.Find("Crew"));
            GameObject spawnedAIUnit = Instantiate(unitAIPrefab, spawnedAIBoat.transform);
            spawnedUnit.GetComponent<AIMovement>().navMeshInterface = spawnedAIUnit.GetComponent<NavMeshUnitInterface>();
            spawnedAIUnit.transform.position += new Vector3(0, 0, numberOfSailors * 2);
            numberOfSailors += 1;
        }

        
        Transform playerAI = boatAiTransform.Find("ECM_BaseFirstPersonControllerAI"); // 1.9 =x, 4.137 = y
        playerAI.parent = spawnedAIBoat.transform;
        playerAI.localPosition = new Vector3(1.9f, 4.137f, 0);
        
        GameObject player = transform.Find("ECM_BaseFirstPersonController").gameObject;
        player.transform.parent = spawnedBoat.transform.Find("Crew");//3.28 = y
        player.GetComponent<PlayerTriggerController>().SetUpBoat(spawnedBoat.GetComponent<BoatControls>());

        Transform cameraTopDown = transform.Find("TopDownCamera");
        cameraTopDown.parent = spawnedBoat.transform;//67=y, .4 = z
        cameraTopDown.localPosition = new Vector3(0, 67f, 0);
        GameObject.Find("CameraWrapper").GetComponent<FollowCameraController>().targetTransform = spawnedBoat.transform.Find("CamTargetTransform");
        GameObject.Find("CameraWrapper/BoatCam").GetComponent<MouseLookBoat>().lookLocation = spawnedBoat.transform.Find("CamTargetTransform");
    }

    public int GetTeam() {
        if (gameObject.layer == 13) {
            return 1;
        }
        if (gameObject.layer == 14) {
            return 2;
        }
        return 0;
    }
}
