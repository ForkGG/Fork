using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Fork.Logic.Persistence;
using ForkCommon.Model.Application;
using ForkCommon.Model.Entity.Pocos.Player;

namespace Fork.Logic.Managers;

public class ApplicationManager : IApplicationManager
{
    private readonly ILogger<ApplicationManager> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public ApplicationManager(ILogger<ApplicationManager> logger, IServiceScopeFactory scopeFactory,
        IConfiguration configuration, IObjectCache objectCache)
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

            {
                objectCache.PlayersByUid = new Dictionary<string, Player>();
                foreach (var player in dbContext.PlayerSet.Where(p => p.IsOfflinePlayer == false))
                {
                    objectCache.PlayersByUid.Add(player.Uid, player);
                }
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