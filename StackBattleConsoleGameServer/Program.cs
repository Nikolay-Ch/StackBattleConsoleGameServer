using Microsoft.Extensions.Options;
using StackBattleConsoleGameServer;

Console.WriteLine($"Main: starting...");

IHost? host = null;
ILogger<Program>? logger = null;
try
{
    host = CreateHostBuilder(args);

    logger = host.Services.GetRequiredService<ILogger<Program>>();

    logger?.LogInformation("Main: Load config done...");
    logger?.LogInformation("Main: Waiting for RunAsync to complete");

    await host.RunAsync();

    logger?.LogInformation("Main: RunAsync has completed");
}
finally
{
    logger?.LogInformation("Main: stopping");

    if (host is IAsyncDisposable d) await d.DisposeAsync();
}

static IHost CreateHostBuilder(string[] args)
{
    var cfgBuilder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostContext, configuration) =>
            { configuration.AddJsonFile("/config/appsettings.json", true, false); })
        .ConfigureLogging((builder) =>
        {
            builder.AddProvider(new BattleLoggerProvider());
        })
        .ConfigureServices((hostContext, services) =>
        {
            services.Configure<MainConfiguration>(hostContext.Configuration.GetSection(nameof(MainConfiguration)));
            services.AddSingleton<IArmyReaderFactory>(e => new ArmyFileReaderFactory(e.GetRequiredService<IOptions<MainConfiguration>>()));
            services.AddSingleton<IArmyFactory>(e => new ArmyFactoryFromJSON());
            services.AddHostedService<Worker>();
        });

    return cfgBuilder.Build();
}

/*var army = new Army { TeamName = "First Team" };
army.UnitDescriptions.Add(1, new UnitDescription { UnidDescriptionId = 1, UnitName = "Infantry", Attack = 1, Defence = 1, HitPoints = 2 });
army.UnitDescriptions.Add(2, new UnitDescription { UnidDescriptionId = 2, UnitName = "Heavy Infantry", Attack = 2, Defence = 2, HitPoints = 2 });
army.UnitDescriptions.Add(3, new UnitDescription { UnidDescriptionId = 3, UnitName = "Knight", Attack = 3, Defence = 3, HitPoints = 10 });

army.Units.Add(new Unit(army.UnitDescriptions[1]));
army.Units.Add(new Unit(army.UnitDescriptions[2]));
army.Units.Add(new Unit(army.UnitDescriptions[3]));
army.Units.Add(new Unit(army.UnitDescriptions[1]));

var str = JsonSerializer.Serialize(army);
var army2 = JsonSerializer.Deserialize<Army>(str);
Console.WriteLine(army.UnitDescriptions.First().Value.Price);
Console.WriteLine(str);*/

