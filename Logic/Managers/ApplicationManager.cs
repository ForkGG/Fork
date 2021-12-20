using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Persistence;
using ProjectAveryCommon.Model.Application;

namespace ProjectAvery.Logic.Managers;

public class ApplicationManager : IApplicationManager
{
    private readonly ILogger<ApplicationManager> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

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

        using (var scope = _scopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            AppSettings = dbContext.ReadAppSettings().Result;

            if (string.IsNullOrWhiteSpace(AppSettings.EntityPath))
            {
                AppSettings.EntityPath = Path.Combine(AppPath, "entities");
#pragma warning disable CS4014
                dbContext.WriteAppSettings(AppSettings);
#pragma warning restore CS4014
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