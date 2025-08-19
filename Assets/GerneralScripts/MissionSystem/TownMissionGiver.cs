using System.Collections.Generic;
using UnityEngine;
using GerneralScripts.MissionSystem;

public class TownMissionGiver : MonoBehaviour
{
    [Header("Town Info")]
    [SerializeField] private string townId;
    [SerializeField] private Transform townCenter;
    [SerializeField, Min(1)] private int townLevel = 1;

    [Header("Available Missions For This Town")]
    [SerializeField] private List<MissionTemplate> availableMissions = new List<MissionTemplate>();

    // Optional: prevent re-assigning same mission multiple times in this session
    private readonly HashSet<string> _assignedMissionIds = new HashSet<string>();

    // Call this on player interaction (e.g., from a UI button or trigger)
    public void AssignRandomMission()
    {
        if (availableMissions == null || availableMissions.Count == 0)
        {
            Debug.LogWarning($"[{nameof(TownMissionGiver)}] No missions configured for town '{townId}'.");
            return;
        }

        // Pick the first unassigned mission; customize to your needs (random/weighted/etc.)
        var template = GetNextUnassignedTemplate();
        if (template == null)
        {
            Debug.Log($"[{nameof(TownMissionGiver)}] All missions already assigned for town '{townId}'.");
            return;
        }

        var context = new TownContext(
            townId: string.IsNullOrEmpty(townId) ? gameObject.name : townId,
            townCenter: townCenter ? townCenter.position : transform.position,
            townLevel: townLevel
        );

        var mission = MissionFactory.CreateMission(template, context);

        // Add to your existing system
        MissionSystem.Instance?.AddMission(mission);

        // If your MissionSystem does not call Initialize(), do it here:
        // mission.Initialize();

        // Broadcast
        MissionEvents.InvokeMissionStarted(mission.MissionID);

        _assignedMissionIds.Add(mission.MissionID);

        Debug.Log($"Assigned mission '{mission.Title}' ({mission.MissionID}) at town '{context.TownId}'.");
    }

    private MissionTemplate GetNextUnassignedTemplate()
    {
        foreach (var t in availableMissions)
        {
            if (!_assignedMissionIds.Contains(t.MissionID))
                return t;
        }
        return null;
    }
}
