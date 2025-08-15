using System;
using MapMode.Scripts;
using UnityEngine;

[Serializable]
public class EnemyKillTask : MissionTask
{
    public int TargetKills { get; private set; }
    public int CurrentKills { get; private set; }
    public string TargetEnemyID { get; private set; }

    public EnemyKillTask(string taskName, int step, int killsToReach, string targetEnemyID)
        : base(taskName, step)
    {
        TargetKills = killsToReach;
        TargetEnemyID = targetEnemyID;

        Debug.Log($"EnemyKillTask created: {TaskName}, Target: {TargetEnemyID}, Kills Needed: {TargetKills}");

        
        CombatEvents.EnemyKilled += HandleEnemyKilled;
    }

    public override void Initialize()
    {

        // Re-subscribe to the enemy killed event
        CombatEvents.EnemyKilled += HandleEnemyKilled;

        Debug.Log($"Initialized EnemyKillTask: {TaskName}, Target: {TargetEnemyID}, Current Kills: {CurrentKills}/{TargetKills}");
    }

    private void HandleEnemyKilled(string enemyID)
    {
        // Check if the task is active, not completed, and the enemy ID matches
        if (!isActive || IsCompleted || enemyID != TargetEnemyID) return;

        CurrentKills++;
        Debug.Log($"Enemy killed: {enemyID}. Progress: {CurrentKills}/{TargetKills}");

        CheckProgress();
    }

    public override void CheckProgress()
    {
        if (CurrentKills >= TargetKills)
        {
            CompleteTask();

            // Unsubscribe from the event when the task is completed
            CombatEvents.EnemyKilled -= HandleEnemyKilled;
        }
    }
    public override void Cleanup()
    {
        // Unsubscribe from the enemy killed event
        CombatEvents.EnemyKilled -= HandleEnemyKilled;
        base.Cleanup(); // Call the base cleanup for event cleanup
    }

    ~EnemyKillTask()
    {
        Cleanup();
    }
}
