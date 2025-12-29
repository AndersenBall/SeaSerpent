namespace GerneralScripts.BattleManagement
{
    public enum BattleSide { A, B }
    public enum ControllerKind { Player, AI }
    public enum ResolutionMode { Auto, Playable } // Playable only when a Player is involved (optional rule)

    public sealed class BattleParticipant
    {
        public string ParticipantId { get; }          // e.g. FleetID or CommanderID
        public ControllerKind Controller { get; }
        public Fleet Fleet { get; }

        public BattleParticipant(string participantId, ControllerKind controller, Fleet fleet)
        {
            ParticipantId = participantId;
            Controller = controller;
            Fleet = fleet;
        }
    }


}