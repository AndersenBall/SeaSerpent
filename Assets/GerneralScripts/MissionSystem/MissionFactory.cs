using System.Collections.Generic;
using System.Linq;

public static class MissionFactory
{
    public static Mission CreateMission(MissionTemplate template, TownContext context)
    {
        var runtimeTasks = new List<MissionTask>(template.Tasks.Count);
        foreach (var def in template.Tasks)
        {
            var task = def.CreateRuntimeTask(context);
            runtimeTasks.Add(task);
        }

        // Construct your runtime mission using existing Mission class
        var mission = new Mission(template.MissionID, template.Title, template.Description, runtimeTasks);

        // Optionally initialize here if your MissionSystem does not do it
        // mission.Initialize();

        return mission;
    }
}