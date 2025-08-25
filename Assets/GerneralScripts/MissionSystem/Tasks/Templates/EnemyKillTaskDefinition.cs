using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Missions/Tasks/Enemy Kill Task Definition", fileName = "EnemyKillTaskDefinition")]
public class EnemyKillTaskDefinition : TaskTemplate
{
 
    [Header("Enemy Kill Settings")]
    [Tooltip("ID of the enemy type to track. Leave empty to count any enemy, if supported by your task.")]
    [SerializeField] private Nation targetNation = Nation.Britain;
    [Min(1)]
    [SerializeField] private int requiredKills = 1;

    public override TaskInstance CreateRuntimeTask()
    {
        return new DefeatNationsFleet(taskName, step, requiredKills, targetNation);
    }

  
    
}