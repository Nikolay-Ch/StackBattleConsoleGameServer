using System.Text;

namespace StackBattleConsoleGameServer;

internal class BattleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new BattleLogger { CategoryName = categoryName };

    public void Dispose() { }
}

internal sealed class BattleLoggerScope : IDisposable
{
    private Action EndScope { get; } = default!;

    public BattleLoggerScope(Action endScope) => EndScope = endScope;

    public void Dispose() => EndScope();
}

internal class BattleLogger : ILogger
{
    public static string DefaultCategoryName = "BattleLogger";

    public string LogFileName { get; set; } = "default.log";
    public string? BattleNumber { get; set; } = null;

    public string CategoryName { get; init; } = default!;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        BattleNumber = state.ToString();
        return new BattleLoggerScope(() => BattleNumber = null);
    }

    public bool IsEnabled(LogLevel logLevel) => true;//logLevel == LogLevel.Information;

    private readonly object fileLock = new();
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel) || CategoryName != DefaultCategoryName)
            return;

        var text = formatter(state, exception);

        if (string.IsNullOrEmpty(text))
            return;

        lock (fileLock)
        {
            using var writer = new StreamWriter(FileName, true, Encoding.UTF8);

            var logRecord = string.Format("{0} [{1}] {2} {3}",
                "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]",
                logLevel.ToString(),
                text, exception != null ? exception.StackTrace : "");

            writer.WriteLine(logRecord);
        }
    }

    private string FileName => LogFileName + (BattleNumber != null ? $"-{BattleNumber}" : "");
}
