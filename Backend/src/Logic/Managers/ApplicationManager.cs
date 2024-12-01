using System;
using System.IO;
using Fork.Logic.Persistence;
using ForkCommon.Model.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Managers;

public class ApplicationManager
{
    public ApplicationManager(ILogger<ApplicationManager> logger, IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        DirectoryInfo directoryInfo = Directory.CreateDirectory(
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ForkApp"));
        AppPath = directoryInfo.FullName;
        logger.LogInformation("Data directory of Fork is: " + AppPath);

        using (IServiceScope scope = scopeFactory.CreateScope())
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
            $"{configuration["UserAgent"]} Version {configuration["Version:Major"]}.{configuration["Version:Minor"]}.{configuration["Version:Patch"]}";
    }

    public string AppPath { get; init; }
    public AppSettings AppSettings { get; }
    public string EntityPath { get; }
    public string UserAgent { get; }
}