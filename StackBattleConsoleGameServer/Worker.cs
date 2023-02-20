using Microsoft.Extensions.Options;

namespace StackBattleConsoleGameServer;

public class Worker : BackgroundService
{
    private ILogger<Worker> Logger { get; }
    private ILoggerFactory LoggerFactory { get; set; }
    public MainConfiguration MainConfiguration { get; }
    public IArmyReaderFactory ArmyReaderFactory { get; }
    public IArmyFactory ArmyFactory { get; }

    public Worker(ILogger<Worker> logger, ILoggerFactory loggerFactory, IOptions<MainConfiguration> mainConfiguration, IArmyReaderFactory armyReaderFactory, IArmyFactory armyFactory)
    {
        Logger = logger;
        LoggerFactory = loggerFactory;
        MainConfiguration = mainConfiguration.Value;
        ArmyReaderFactory = armyReaderFactory;
        ArmyFactory = armyFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        /*while (!stoppingToken.IsCancellationRequested)
        {
            Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }*/

        var army1 = ArmyFactory.CreateArmy(ArmyReaderFactory.GetFirstArmyReader().ReadArmyData());
        var army2 = ArmyFactory.CreateArmy(ArmyReaderFactory.GetSecondArmyReader().ReadArmyData());

        var battleLogger = LoggerFactory.CreateLogger("BattleLogger");
        var battleField = new Battlefield { Logger = battleLogger, Army1 = army1, Army2 = army2 };
        var score = await battleField.DoCompetition();

        foreach (var (teamName, pointCount) in score)
            Console.WriteLine($"Team '{teamName}' got {pointCount} points");
    }
}
