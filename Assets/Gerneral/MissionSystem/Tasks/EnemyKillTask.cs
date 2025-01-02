using System;
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


        GameEvents.OnEnemyKilled += HandleEnemyKilled;
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
            GameEvents.OnEnemyKilled -= HandleEnemyKilled;
        }
    }

    ~EnemyKillTask()
    {
        // Ensure cleanup in case the task is destroyed before completion
        GameEvents.OnEnemyKilled -= HandleEnemyKilled;
    }
}
