using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Missions/Tasks/Enemy Kill Task Definition", fileName = "EnemyKillTaskDefinition")]
public class EnemyKillTaskDefinition : TaskDefinition
{
    [Header("Task Meta")]
    [SerializeField] private string taskName = "Eliminate Targets";

    [Header("Enemy Kill Settings")]
    [Tooltip("ID of the enemy type to track. Leave empty to count any enemy, if supported by your task.")]
    [SerializeField] private string targetEnemyId = "";
    [Min(1)]
    [SerializeField] private int requiredKills = 1;
    
    public override MissionTask CreateTask(int step)
    {

        return new EnemyKillTask(taskName, step, targetEnemyId, requiredKills);
        
    }
}