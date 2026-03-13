using System.Collections.Generic;
using System.Linq;
using GerneralScripts.BattleManagement;
using GerneralScripts.Utils;
using MapMode.Scripts.PostBattle;
using UnityEngine;

public class BoatMaster : MonoBehaviour
{
    public int tileSize = 25;
    private BoatTeamManager[] boatTeamManagers;

    private readonly List<string> sunkEnemyBoatNames = new();
    private readonly List<Boat> initialEnemyBoats = new();
    private readonly List<Boat> initialPlayerBoats = new();
    private bool combatEnded;

    private Fleet playerFleet;
    private Fleet enemyFleet;
    private BattleSession activeSession;

    List<BoatAI> allBoatsList;

    private void Awake()
    {
        ResolveBattleContext();
        CacheInitialBoats();
    }

    void Start()
    {
        boatTeamManagers = gameObject.GetComponentsInChildren<BoatTeamManager>();

        SpawnBoats();
        allBoatsList = new List<BoatAI>(gameObject.GetComponentsInChildren<BoatAI>());
        Debug.Log("Log:BoatMaster:total boats count:" + allBoatsList.Count());

    }

    private void ResolveBattleContext()
    {
        activeSession = BattleManager.Instance?.CurrentSession;
        if (activeSession == null)
        {
            Debug.LogError("BoatMaster requires an active BattleSession from BattleManager.");
            enabled = false;
            return;
        }

        if (activeSession.PlayerSide == BattleSide.A)
        {
            playerFleet = activeSession.SideA.Fleet;
            enemyFleet = activeSession.SideB.Fleet;
        }
        else
        {
            playerFleet = activeSession.SideB.Fleet;
            enemyFleet = activeSession.SideA.Fleet;
        }

        if (playerFleet == null || enemyFleet == null)
        {
            Debug.LogError("BoatMaster resolved a BattleSession with missing fleet references.");
            enabled = false;
        }
    }

    private void CacheInitialBoats()
    {
        initialPlayerBoats.Clear();
        if (playerFleet?.GetBoats() != null)
            initialPlayerBoats.AddRange(playerFleet.GetBoats());

        initialEnemyBoats.Clear();
        if (enemyFleet?.GetBoats() != null)
            initialEnemyBoats.AddRange(enemyFleet.GetBoats());
    }

    private void SpawnBoats()
    {

        if (playerFleet?.GetBoats() != null) {
            var teamOneBoatTeam = boatTeamManagers.FirstOrDefault(boatTeam => boatTeam.GetTeam() == 1);
            if (teamOneBoatTeam == null)
            {
                Debug.LogError("BoatMaster missing team 1 BoatTeamManager.");
                return;
            }

            var boats = playerFleet.GetBoats();
            int flagshipIndex = boats.FindIndex(boat => boat.boatName == playerFleet.FlagShipName);

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

        if (enemyFleet != null) {
            foreach (Boat b in enemyFleet.GetBoats()) {
                foreach (BoatTeamManager boatTeam in boatTeamManagers) {
                    if (2 == boatTeam.GetTeam()) {
                        Debug.Log("team:" + boatTeam.GetTeam());
                        boatTeam.SpawnBoat(b);
                    }
                }
            }
        }

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

    public void DestroyBoat(BoatAI boat) {
        if (boat.GetTeamNumber() == 1) {
            Debug.Log("Deleted:" + playerFleet.CommanderName + boat.name);
            playerFleet.RemoveBoat(boat.name);
        }
        else {
            Debug.Log("Destroyed:" + enemyFleet.CommanderName + boat.name);
            sunkEnemyBoatNames.Add(boat.name);
            enemyFleet.RemoveBoat(boat.name);
        }

        Debug.Log("boat removed from boat list?:" + allBoatsList.Remove(boat));

        EvaluateCombatEnd();
    }

    private void EvaluateCombatEnd()
    {
        if (combatEnded)
        {
            return;
        }
        var playerAliveCount = GetTeamBoats(1)?.Length ?? 0;
        var enemyAliveCount = GetTeamBoats(2)?.Length ?? 0;

        if (playerAliveCount > 0 && enemyAliveCount > 0)
        {
            return;
        }

        combatEnded = true;
        ActivatePostBattleLooting();
    }

    public void ActivatePostBattleLooting()
    {
        UpdatePlayerFleet();
        UpdateEnemyFleet();

        if (playerFleet == null || playerFleet.GetBoats().Count == 0)
        {
            PlayerStateService.TrySpendMoney(PlayerStateService.Money / 2f);
            var resultOnDefeat = BuildBattleResult(playerDefeated: true);
            BattleManager.Instance.SubmitCombatResult(resultOnDefeat);
            return;
        }

        var postCombatData = BuildPostCombatData();
        var battleResult = BuildBattleResult(playerDefeated: false);
        PostCombatFlowService.BeginPostCombat(postCombatData, battleResult);
    }

    private PostCombatData BuildPostCombatData()
    {
        var survivingEnemyBoats = enemyFleet?.GetBoats() ?? new List<Boat>();

        var sunkEnemyBoats = initialEnemyBoats
            .Where(boat => sunkEnemyBoatNames.Contains(boat.boatName))
            .ToList();

        var loot = LootUtils.ComputeAvailableLoot(
            survivingEnemyBoats,
            sunkEnemyBoats,
            sunkLootFraction: 0.25f,
            out int goldGained);

        return new PostCombatData(
            enemyFleet,
            survivingEnemyBoats,
            loot,
            goldGained);
    }

    private BattleResult BuildBattleResult(bool playerDefeated)
    {
        return BattleResultFactory.BuildFromPlayableCombat(
            activeSession,
            initialPlayerBoats,
            playerFleet?.GetBoats(),
            initialEnemyBoats,
            enemyFleet?.GetBoats(),
            playerDefeated);
    }

    public void EndBattle() {
        // Retained for compatibility with existing references.
        ActivatePostBattleLooting();
    }

    private void UpdatePlayerFleet() {
        BoatAI[] allyBoatsAI = GetTeamBoats(1);
        List<Boat> allyBoatsData = playerFleet.GetBoats();

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
        playerFleet.SetBoats(allyBoatsData);
    }

    private void UpdateEnemyFleet() {
        BoatAI[] enemyBoatsAI = GetTeamBoats(2);
        List<Boat> enemyBoatsData = enemyFleet.GetBoats();

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
        enemyFleet.SetBoats(enemyBoatsData);
    }
}
