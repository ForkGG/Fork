using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Services.FileServices;
using ProjectAvery.Util.ExtensionMethods;
using ProjectAveryCommon.ExtensionMethods;
using ProjectAveryCommon.Model.Application;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.Automation;
using ProjectAveryCommon.Model.Entity.Pocos.Player;
using ProjectAveryCommon.Model.Entity.Pocos.ServerSettings;

namespace ProjectAvery.Logic.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> _logger;
        private readonly IFileReaderService _fileReader;

        public DbSet<Server> ServerSet { get; set; }
        public DbSet<Player> PlayerSet { get; set; }
        public DbSet<SettingsKeyValue> AppSettingsSet { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger, IFileReaderService fileReader) : base(options)
        {
            _logger = logger;
            _fileReader = fileReader;
        }

        public async Task<AppSettings> ReadAppSettings()
        {
            AppSettings result = new AppSettings();
            foreach (var setting in result)
            {
                var entry = AppSettingsSet.FirstOrDefault(s => s.Key == setting.Key);
                if (entry != null)
                {
                    var property = result.GetType().GetProperty(setting.Key);
                    if (property != null && property.CanWrite)
                    {
                        var type = property.PropertyType;
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
            var writtenKeys = new List<string>();
            
            // Add or Update all keys in the settings
            foreach (var setting in settings)
            {
                var entry = AppSettingsSet.FirstOrDefault(s => s.Key == setting.Key);
                
                writtenKeys.Add(setting.Key);
                if (entry == null)
                {
                    entry = new SettingsKeyValue { Key = setting.Key };
                    await AppSettingsSet.AddAsync(entry);
                }
                entry.Value = setting.Value;
            }
            
            // Remove keys which got removed
            AppSettingsSet.RemoveRange(AppSettingsSet.Where(s => !writtenKeys.Contains(s.Key)));
            
            await SaveChangesAsync();
        }
    }
}