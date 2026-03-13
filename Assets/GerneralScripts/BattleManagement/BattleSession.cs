namespace GerneralScripts.BattleManagement
{
    public sealed class BattleSession
    {
        public string SessionId { get; } = System.Guid.NewGuid().ToString("N");
    
        public BattleParticipant SideA { get; }
        public BattleParticipant SideB { get; }
    
        public ResolutionMode Mode { get; }
        public string ReturnSceneName { get; }
    
        public BattleSession(BattleParticipant a, BattleParticipant b, ResolutionMode mode, string returnSceneName)
        {
            SideA = a;
            SideB = b;
            Mode = mode;
            ReturnSceneName = returnSceneName;
        }
    
        public bool HasPlayer => SideA.Controller == ControllerKind.Player || SideB.Controller == ControllerKind.Player;
        public BattleSide PlayerSide =>
            SideA.Controller == ControllerKind.Player ? BattleSide.A : BattleSide.B;
    }

}