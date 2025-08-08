using System;
using MapMode.Scripts.NavalInteractionContracts;

namespace MapMode.Scripts
{
    public class NavalInteractionEvent
    {
        public static event Action<AttackFleetEvent> AttackedFleet;

        public static void InvokeAttackedFleet(AttackFleetEvent attackEvent)
        {
            AttackedFleet?.Invoke(attackEvent);
        }

        public static void Clear()
        {
            AttackedFleet = null;
        }
    }
}