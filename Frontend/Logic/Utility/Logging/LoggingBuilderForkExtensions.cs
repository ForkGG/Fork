using Microsoft.Extensions.Options;

namespace ForkFrontend.Logic.Utility.Logging;

public static class LoggingBuilderForkExtensions
{
    public static ILoggingBuilder AddFork(this ILoggingBuilder loggingBuilder, Action<ForkLoggerOptions> configure)
    {
        loggingBuilder.Services.Configure(configure);
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ForkLoggerProvider>(services =>
        {
            IOptions<ForkLoggerOptions> options = services.GetRequiredService<IOptions<ForkLoggerOptions>>();
            return new ForkLoggerProvider(options);
        });
        return loggingBuilder;
    }
}