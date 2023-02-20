using Microsoft.Extensions.Options;

namespace StackBattleConsoleGameServer;

public class ArmyFileReaderFactory : IArmyReaderFactory
{
    public MainConfiguration MainConfiguration { get; }

    public ArmyFileReaderFactory(IOptions<MainConfiguration> mainConfiguration)
    {
        MainConfiguration = mainConfiguration.Value;
    }

    public IArmyReader GetFirstArmyReader() => new ArmyFileReader { FileName = MainConfiguration.ArmyFileTeam1 };
    public IArmyReader GetSecondArmyReader() => new ArmyFileReader { FileName = MainConfiguration.ArmyFileTeam2 };
}
internal class ArmyFileReader : IArmyReader
{
    private bool disposedValue;

    public string FileName { get; init; } = "";

    public Stream? Stream { get; private set; }

    public Stream ReadArmyData() => Stream = File.OpenRead(FileName);

    #region Dispose
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                Stream?.Dispose();

            disposedValue = true;
        }
    }

    ~ArmyFileReader() => Dispose(disposing: false);

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
