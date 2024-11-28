namespace ForkFrontend.Logic.Utility.Logging;

public class ForkLogger : ILogger
{
    private readonly ForkLoggerOptions _options;

    public ForkLogger(ForkLoggerOptions options)
    {
        _options = options;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        //Console.WriteLine(formatter(state, exception));
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}