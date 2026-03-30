public interface IFleetAIPolicy
{
    FleetAIState GetInitialState(FleetMapController controller);
    bool CanEngage(FleetMapController controller, Fleet otherFleet);
    void Tick(FleetMapController controller);
    void UpdateAfterEncounter(FleetMapController controller, FleetMapController otherFleetController, bool wonBattle);
}

public static class FleetAIPolicyFactory
{
    public static IFleetAIPolicy CreateFor(AIFleetType fleetType)
    {
        switch (fleetType)
        {
            case AIFleetType.Trade:
                return new TradeFleetAIPolicy();
            case AIFleetType.Pirate:
                return new PirateFleetAIPolicy();
            case AIFleetType.NavalPatrol:
                return new NavalPatrolFleetAIPolicy();
            case AIFleetType.War:
                return new WarFleetAIPolicy();
            default:
                return new DefaultFleetAIPolicy();
        }
    }
}

public class DefaultFleetAIPolicy : IFleetAIPolicy
{
    public virtual FleetAIState GetInitialState(FleetMapController controller) => FleetAIState.Idle;
    public virtual bool CanEngage(FleetMapController controller, Fleet otherFleet) => false;
    public virtual void Tick(FleetMapController controller) { }
    public virtual void UpdateAfterEncounter(FleetMapController controller, FleetMapController otherFleetController, bool wonBattle)
    {
        controller.ClearCurrentTarget();
        controller.ChangeState(FleetAIState.Idle);
    }
}

public class TradeFleetAIPolicy : DefaultFleetAIPolicy
{
    public override FleetAIState GetInitialState(FleetMapController controller) => FleetAIState.TransitToTown;

    public override bool CanEngage(FleetMapController controller, Fleet otherFleet)
    {
        return otherFleet != null && otherFleet.FleetType == AIFleetType.Pirate;
    }

    public override void Tick(FleetMapController controller)
    {
        if (controller.CurrentState == FleetAIState.TransitToTown)
        {
            FleetMapController threat = controller.FindNearestHostileFleet();
            if (threat != null && controller.ShouldFleeFrom(threat))
            {
                controller.SetCurrentTarget(threat);
                controller.ChangeState(FleetAIState.Flee);
            }
            return;
        }

        if (controller.CurrentState == FleetAIState.Flee || controller.CurrentState == FleetAIState.Regroup)
        {
            controller.SetDestinationToNearestFriendlyTown();
        }
    }

    public override void UpdateAfterEncounter(FleetMapController controller, FleetMapController otherFleetController, bool wonBattle)
    {
        controller.ClearCurrentTarget();
        if (!wonBattle)
        {
            controller.ChangeState(FleetAIState.Flee);
            return;
        }

        controller.ChangeState(FleetAIState.TransitToTown);
    }
}

public class PirateFleetAIPolicy : DefaultFleetAIPolicy
{
    public override FleetAIState GetInitialState(FleetMapController controller) => FleetAIState.Search;

    public override bool CanEngage(FleetMapController controller, Fleet otherFleet)
    {
        return otherFleet != null &&
               (otherFleet.FleetType == AIFleetType.Trade ||
                otherFleet.FleetType == AIFleetType.NavalPatrol ||
                otherFleet.FleetType == AIFleetType.War);
    }

    public override void Tick(FleetMapController controller)
    {
        HandleSearchAndIntercept(controller);
    }

    public override void UpdateAfterEncounter(FleetMapController controller, FleetMapController otherFleetController, bool wonBattle)
    {
        controller.ClearCurrentTarget();
        FleetMapController newTarget = controller.FindNearestHostileFleet();
        if (newTarget != null)
        {
            controller.SetCurrentTarget(newTarget);
            controller.ChangeState(FleetAIState.InterceptTarget);
            return;
        }

        controller.ChangeState(FleetAIState.Search);
    }

    protected void HandleSearchAndIntercept(FleetMapController controller)
    {
        if (controller.CurrentState == FleetAIState.Search || controller.CurrentState == FleetAIState.PatrolRoute)
        {
            FleetMapController target = controller.FindNearestHostileFleet();
            if (target != null)
            {
                controller.SetCurrentTarget(target);
                controller.ChangeState(FleetAIState.InterceptTarget);
            }

            return;
        }

        if (controller.CurrentState == FleetAIState.InterceptTarget)
        {
            if (!controller.HasValidCurrentTarget())
            {
                controller.ClearCurrentTarget();
                controller.ChangeState(FleetAIState.Search);
                return;
            }

            controller.SyncDestinationToCurrentTarget();

            if (controller.GetFleetHealthRatio() < controller.RegroupHpThreshold)
            {
                controller.ChangeState(FleetAIState.Regroup);
            }

            return;
        }

        if (controller.CurrentState == FleetAIState.Regroup || controller.CurrentState == FleetAIState.Flee)
        {
            controller.SetDestinationToNearestFriendlyTown();
        }
    }
}

public class NavalPatrolFleetAIPolicy : PirateFleetAIPolicy
{
    public override FleetAIState GetInitialState(FleetMapController controller) => FleetAIState.PatrolRoute;

    public override bool CanEngage(FleetMapController controller, Fleet otherFleet)
    {
        return otherFleet != null && otherFleet.FleetType == AIFleetType.Pirate;
    }
}

public class WarFleetAIPolicy : PirateFleetAIPolicy
{
    public override FleetAIState GetInitialState(FleetMapController controller) => FleetAIState.PatrolRoute;

    public override bool CanEngage(FleetMapController controller, Fleet otherFleet)
    {
        if (otherFleet == null || controller.GetFleet() == null)
        {
            return false;
        }

        if (otherFleet.FleetType == AIFleetType.Pirate)
        {
            return true;
        }

        return otherFleet.FleetType == AIFleetType.War && otherFleet.Nationality != controller.GetFleet().Nationality;
    }
}
