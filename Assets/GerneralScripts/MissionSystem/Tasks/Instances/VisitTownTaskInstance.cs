using System;


namespace GerneralScripts.MissionSystem.Tasks
{
    [Serializable]
    public class VisitTownTaskInstance : TaskInstance
    {
        public string TargetTownName { get; private set; }

        public VisitTownTaskInstance(string taskName, string targetTownName, int step = 0) : base(taskName, step)
        {
            TargetTownName = targetTownName;
        }

        public override void Initialize()
        {
            TownEvents.TownVisited += HandleTownVisit;
        }

        public override void CheckProgress()
        {
            return;
        }

        public override void Cleanup()
        {
            base.Cleanup();
            TownEvents.TownVisited -= HandleTownVisit;
        }

        private void HandleTownVisit(Town town)
        {
            if (isActive && town.name == TargetTownName)
            {
                CompleteTask();
            }
        }
    }
}