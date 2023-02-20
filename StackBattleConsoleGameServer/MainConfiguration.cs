using System.Reflection;

namespace StackBattleConsoleGameServer;

public class MainConfiguration
{
    public string ArmyFileTeam1 { get; init; } = "";
    public string ArmyFileTeam2 { get; init; } = "";
    public string PathLogs { get; init; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
}
