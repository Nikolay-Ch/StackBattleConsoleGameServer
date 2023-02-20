using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;

namespace StackBattleConsoleGameServer;

internal class Battlefield
{
    public ILogger Logger { get; init; } = default!;
    public IArmy Army1 { get; init; } = default!;
    public IArmy Army2 { get; init; } = default!;

    public async Task<IEnumerable<(string teamName, int pointCount)>> DoCompetition()
    {
        var winners = new List<(string teamName, int pointCount)>();

        using (var scope = Logger.BeginScope("1"))
            winners.AddRange(await FightToEnd(Army1.GetCopy(), Army2.GetCopy()));

        using (var scope = Logger.BeginScope("2"))
            winners.AddRange(await FightToEnd(Army2.GetCopy(), Army1.GetCopy()));

        using (var scope = Logger.BeginScope("3"))
            winners.AddRange(await FightToEnd(Army1.GetCopy(), Army2.GetCopy()));

        using (var scope = Logger.BeginScope("4"))
            winners.AddRange(await FightToEnd(Army2.GetCopy(), Army1.GetCopy()));

        var score = winners
            .GroupBy(e => e.teamName)
            .Select(e => (e.Key, e.Sum(p => p.pointCount))).ToArray();

        using (var scope = Logger.BeginScope("summary"))
            foreach (var (teamName, pointCount) in score)
                Logger.LogInformation("Team '{teamName}' got {pointCount} points", teamName, pointCount);

        return score;
    }

    public async Task<(string teamName, int pointCount)[]> FightToEnd(IArmy army1, IArmy army2)
    {
        using var cts = new CancellationTokenSource(10000);
        var ct = cts.Token;

        var task = Task.Run(new Func<string>(() =>
        {
            // while any alive unit presense
            while (army1.Units.Any() && army2.Units.Any())
            {
                army2.Units[0].GetHit(army1.Units[0], Logger);
                if (army2.Units[0].CurrentHitPoint > 0)
                    army1.Units[0].GetHit(army2.Units[0], Logger);

                if (army1.Units[0].CurrentHitPoint == 0)
                    army1.Units.RemoveAt(0);

                if (army2.Units[0].CurrentHitPoint == 0)
                    army2.Units.RemoveAt(0);

                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }

            return army1.Units.Any() ? army1.TeamName : army2.TeamName;
        }), ct);

        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Timeout exeeded - Draw!");
            return new[] { (army1.TeamName, 1), (army2.TeamName, 1) };
        }
        finally
        {
            cts.Dispose();
        }

        Logger.LogInformation("Team '{winTeam}' - Wins!", task.Result);
        return new[] { (task.Result, 3) };
    }
}
