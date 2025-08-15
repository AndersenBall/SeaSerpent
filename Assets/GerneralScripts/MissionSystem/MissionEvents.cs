using System;

namespace GerneralScripts.MissionSystem
{
    public class MissionEvents
    {
        public static event Action<string> MissionCompleted;
        public static event Action<string> MissionStarted;

        public static void InvokeMissionCompleted(string missionID)
        {
            MissionCompleted?.Invoke(missionID);
        }

        public static void InvokeMissionStarted(string missionID)
        {
            MissionStarted?.Invoke(missionID);
        }

        public static void Clear()
        {
            MissionCompleted = null;
            MissionStarted = null;
        }
    }
}