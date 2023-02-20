using System.Text.Json.Serialization;

namespace StackBattleConsoleGameServer;

public enum UnitType
{
    Ordinary = 0,
    Shooting = 2,
    Healer = 3,
    Squire = 4,
    Necromancer = 5
}

public class UnitDescription
{
	public int UnidDescriptionId { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public UnitType UnitType { get; init; }

    public string UnitName { get; init; } = default!;

    public int Attack { get; init; }

	public int Defence { get; init; }

	public int HitPoints { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int SpecialForcePower { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int SpecialForceDistance { get; init; }

    private int _specialForceChance;
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int SpecialForceChance
    {
        get { return _specialForceChance; }
        init
        {
            if (value < 0 || value > 100)
                throw new ArgumentException();

            _specialForceChance = value;
        }
    }

    [JsonIgnore]
	public int Price => Attack + Defence + HitPoints + (SpecialForceChance > 0 ? (SpecialForcePower * SpecialForceDistance) / SpecialForceChance : 0);

    [JsonIgnore]
    public static UnitDescription EmptyUnitDescription { get; } = new UnitDescription { };
}
