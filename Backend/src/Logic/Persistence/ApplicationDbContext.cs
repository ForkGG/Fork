using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fork.Logic.Services.FileServices;
using ForkCommon.Model.Application;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly FileReaderService _fileReader;
    private readonly ILogger<ApplicationDbContext> _logger;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger,
        FileReaderService fileReader) : base(options)
    {
        _logger = logger;
        _fileReader = fileReader;
    }

    public DbSet<Server> ServerSet { get; set; }
    public DbSet<Player> PlayerSet { get; set; }
    public DbSet<SettingsKeyValue> AppSettingsSet { get; set; }

    public async Task<AppSettings> ReadAppSettings()
    {
        AppSettings result = new();
        foreach (SettingsKeyValue setting in result)
        {
            if (setting.Key == null)
            {
                continue;
            }

            SettingsKeyValue? entry = AppSettingsSet.FirstOrDefault(s => s.Key == setting.Key);
            if (entry != null)
            {
                PropertyInfo? property = result.GetType().GetProperty(setting.Key);
                if (property != null && property.CanWrite)
                {
                    Type type = property.PropertyType;
                    if (type == typeof(int) && int.TryParse(entry.Value, out int parsedInt))
                    {
                        try
                        {
                            property.SetValue(result, parsedInt);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"Failed to write app setting {property.Name}");
                        }
                    }
                    else if (type == typeof(bool) && bool.TryParse(entry.Value, out bool parsedBool))
                    {
                        try
                        {
                            property.SetValue(result, parsedBool);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"Failed to write app setting {property.Name}");
                        }
                    }
                    else
                    {
                        try
                        {
                            property.SetValue(result, entry.Value);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"Failed to write app setting {property.Name}");
                        }
                    }
                }
            }
        }

        if (AppSettingsSet.Count() != result.Count())
        {
            await WriteAppSettings(result);
        }

        return result;
    }

    public async Task WriteAppSettings(AppSettings settings)
    {
        List<string> writtenKeys = new();

        // Add or Update all keys in the settings
        foreach (SettingsKeyValue setting in settings)
        {
            if (setting.Key == null)
            {
                continue;
            }

            SettingsKeyValue? entry = AppSettingsSet.FirstOrDefault(s => s.Key == setting.Key);
            writtenKeys.Add(setting.Key);

            if (entry == null)
            {
                entry = new SettingsKeyValue { Key = setting.Key };
                await AppSettingsSet.AddAsync(entry);
            }

            entry.Value = setting.Value;
        }

        // Remove keys which got removed
        AppSettingsSet.RemoveRange(AppSettingsSet.Where(s => s.Key != null && !writtenKeys.Contains(s.Key)));

        await SaveChangesAsync();
    }
}