using System.Text.Json;

namespace StackBattleConsoleGameServer;

public interface IArmyFactory
{
    IArmy CreateArmy(Stream stream);
}

public class ArmyFactoryFromJSON : IArmyFactory
{
    public IArmy CreateArmy(Stream stream) => JsonSerializer.Deserialize<Army>(stream)!;
}
