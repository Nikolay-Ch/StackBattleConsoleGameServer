namespace StackBattleConsoleGameServer;

public interface IArmyReaderFactory
{
    IArmyReader GetFirstArmyReader();
    IArmyReader GetSecondArmyReader();
}
