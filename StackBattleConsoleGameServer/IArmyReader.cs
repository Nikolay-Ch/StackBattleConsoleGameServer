namespace StackBattleConsoleGameServer;

public interface IArmyReader : IDisposable
{
    Stream ReadArmyData();
}
