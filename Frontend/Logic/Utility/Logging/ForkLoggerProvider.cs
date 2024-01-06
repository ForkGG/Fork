using Microsoft.Extensions.Options;

namespace ForkFrontend.Logic.Utility.Logging;

public class ForkLoggerProvider : ILoggerProvider
{
    private readonly ForkLoggerOptions _options;

    public ForkLoggerProvider(IOptions<ForkLoggerOptions> options)
    {
        _options = options.Value;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new ForkLogger(_options);
    }

    public void Dispose()
    {
    }
}