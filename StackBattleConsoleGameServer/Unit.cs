namespace StackBattleConsoleGameServer;

public interface IUnit
{
    UnitDescription UnitDescription { get; }

    int CurrentHitPoint { get; }

    void GetHit(IUnit enemyUnit, ILogger logger);
    IUnit LocalCopyState();
}

public class Unit : IUnit
{
    public UnitDescription UnitDescription { get; }

    public int CurrentHitPoint { get; protected set; }

    public void GetHit(IUnit enemyUnit, ILogger logger)
    {
        var HitPointBeforeHit = CurrentHitPoint;
        CurrentHitPoint -= enemyUnit.UnitDescription.Attack - UnitDescription.Defence;
        if (CurrentHitPoint < 0)
            CurrentHitPoint = 0;

        logger.LogTrace("Unit: {Unit1Name} with Defence: {Unit1Defense} and HP: {Unit1SourceHP}" +
            " was hit by unit: {Unit2Name} with Attack: {Unit2Attack}." +
            " Now the hitted unit has HP: {Unit1HP}",
            UnitDescription.UnitName, UnitDescription.Defence, HitPointBeforeHit,
            enemyUnit.UnitDescription.UnitName, enemyUnit.UnitDescription.Attack, CurrentHitPoint);
    }

    public Unit(UnitDescription unitDescription)
    {
        UnitDescription = unitDescription;
        CurrentHitPoint = UnitDescription.HitPoints;
    }

    public IUnit LocalCopyState() => new Unit(UnitDescription) { CurrentHitPoint = CurrentHitPoint };
}
