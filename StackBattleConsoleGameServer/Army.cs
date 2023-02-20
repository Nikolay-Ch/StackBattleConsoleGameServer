using System.Text.Json.Serialization;

namespace StackBattleConsoleGameServer;

public interface IArmy
{
	string TeamName { get; set; }
    public Dictionary<int, UnitDescription> UnitDescriptions { get; }
    public List<IUnit> Units { get; }
    public int ArmyPrice { get; }
    public IArmy GetCopy();
}

[JsonConverter(typeof(ArmyConverter))]
public class Army : IArmy
{
	public string TeamName { get; set; } = "";

	public Dictionary<int, UnitDescription> UnitDescriptions { get; init; } = new Dictionary<int, UnitDescription>();

	public List<IUnit> Units { get; init; } = new List<IUnit>();

	public int ArmyPrice => Units.Sum(e => e.UnitDescription.Price);

    public IArmy GetCopy() =>
        new Army
        {
            TeamName = TeamName,
            UnitDescriptions = UnitDescriptions,
            Units = Units.Select(e => e.LocalCopyState()).ToList()
        };
}
