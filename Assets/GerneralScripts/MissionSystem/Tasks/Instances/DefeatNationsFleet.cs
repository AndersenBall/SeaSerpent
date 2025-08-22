using System;
using MapMode.Scripts;
using UnityEngine;

[Serializable]
public class DefeatNationsFleet : TaskInstance
{
    public int TargetKills { get; private set; }
    public int FleetsDefeated { get; private set; }
    public Nation TargetNationality { get; private set; }

    public DefeatNationsFleet(string taskName, int step, int killsToReach, Nation targetEnemyNation)
        : base(taskName, step)
    {
        TargetKills = killsToReach;
        TargetNationality = targetEnemyNation;

        Debug.Log($"EnemyKillTask created: {TaskName}, Target: {targetEnemyNation}, Kills Needed: {TargetKills}");
        CombatEvents.DefeatFleet += HandleDefeatFleet;
    }

    public override void Initialize()
    {
        
        CombatEvents.DefeatFleet += HandleDefeatFleet;

        Debug.Log($"Initialized EnemyKillTask: {TaskName}, Target: {TargetNationality}, Current Kills: {FleetsDefeated}/{TargetKills}");
    }

    private void HandleDefeatFleet(Fleet enemyFleet)
    {
        // Check if the task is active, not completed, and the enemy ID matches
        if (!isActive || IsCompleted || enemyFleet.Nationality != TargetNationality) return;

        FleetsDefeated++;
        Debug.Log($"Enemy killed: {enemyFleet.Nationality}. Progress: {FleetsDefeated}/{TargetKills}");

        CheckProgress();
    }

    public override void CheckProgress()
    {
        if (FleetsDefeated >= TargetKills)
        {
            CompleteTask();

            // Unsubscribe from the event when the task is completed
            CombatEvents.DefeatFleet -= HandleDefeatFleet;
        }
    }
    public override void Cleanup()
    {
        // Unsubscribe from the enemy killed event
        CombatEvents.DefeatFleet -= HandleDefeatFleet;
        base.Cleanup(); 
    }

    ~DefeatNationsFleet()
    {
        Cleanup();
    }
}
