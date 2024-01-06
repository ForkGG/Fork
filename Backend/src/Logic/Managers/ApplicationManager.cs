using System;
using System.IO;
using Fork.Logic.Persistence;
using ForkCommon.Model.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Managers;

public class ApplicationManager : IApplicationManager
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApplicationManager> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public ApplicationManager(ILogger<ApplicationManager> logger, IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _configuration = configuration;

        DirectoryInfo directoryInfo = Directory.CreateDirectory(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ForkApp"));
        AppPath = directoryInfo.FullName;
        _logger.LogInformation("Data directory of Fork is: " + AppPath);

        using (IServiceScope scope = _scopeFactory.CreateScope())
        {
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            AppSettings = dbContext.ReadAppSettings().Result;

            if (string.IsNullOrWhiteSpace(AppSettings.EntityPath))
            {
                AppSettings.EntityPath = Path.Combine(AppPath, "entities");
                _ = dbContext.WriteAppSettings(AppSettings);
            }
        }

        EntityPath = AppSettings.EntityPath;

        UserAgent =
            $"{_configuration["UserAgent"]} Version {_configuration["Version:Major"]}.{_configuration["Version:Minor"]}.{_configuration["Version:Patch"]}";
    }

    public string AppPath { get; init; }
    public AppSettings AppSettings { get; }
    public string EntityPath { get; }
    public string UserAgent { get; }
}