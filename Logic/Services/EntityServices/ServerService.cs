using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAvery.Logic.Notification;
using ProjectAvery.Logic.Persistence;
using ProjectAvery.Logic.Services.FileServices;
using ProjectAvery.Logic.Services.WebServices;
using ProjectAvery.Util.ExtensionMethods;
using ProjectAvery.Util.JavaUtils;
using ProjectAveryCommon.Model.Application.Exceptions;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Enums.Console;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.ServerSettings;
using ProjectAveryCommon.Model.Entity.Transient.Console;
using ProjectAveryCommon.Model.Notifications.EntityNotifications;

namespace ProjectAvery.Logic.Services.EntityServices;

public class ServerService : IServerService
{
    private readonly ILogger<ServerService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IApplicationManager _application;
    private readonly IConsoleService _console;
    private readonly IDownloadService _download;
    private readonly IFileWriterService _fileWriter;
    private readonly INotificationCenter _notificationCenter;

    public ServerService(ILogger<ServerService> logger, ApplicationDbContext context, IApplicationManager application,
        IConsoleService console, IDownloadService download, IFileWriterService fileWriter, INotificationCenter notificationCenter)
    {
        _logger = logger;
        _context = context;
        _application = application;
        _console = console;
        _download = download;
        _fileWriter = fileWriter;
        _notificationCenter = notificationCenter;
    }


    public async Task<ulong> CreateServerAsync(string serverName, ServerVersion serverVersion, VanillaSettings settings,
        JavaSettings javaSettings, string worldPath)
    {
        serverName = RefineName(serverName);
        string serverPath = Path.Combine(_application.EntityPath, serverName);
        if (string.IsNullOrEmpty(settings.LevelName))
        {
            settings.LevelName = "world";
        }

        DirectoryInfo directoryInfo = Directory.CreateDirectory(serverPath);
        // TODO CKE serverVersion.Build = await VersionManager.Instance.GetLatestBuild(serverVersion);
        Server server = new Server(serverName, serverVersion, settings, javaSettings);
        _context.ServerSet.Add(server);

        //Download server.jar
        var downloadProgress = new Progress<float>();
        downloadProgress.ProgressChanged += (_, f) =>
        {
            // TODO CKE send notification update
        };
        await _download.DownloadJarAsync(server, downloadProgress, CancellationToken.None);

        //Move World Files
        if (worldPath != null)
        {
            //TODO CKE add world import
            //new FileImporter().DirectoryCopy(worldPath,
            //   Path.Combine(directoryInfo.FullName, server.VanillaSettings.LevelName), true);
        }

        //Writing necessary files
        await _fileWriter.WriteEula(Path.Combine(_application.EntityPath, directoryInfo.Name));
        await _fileWriter.WriteServerSettings(Path.Combine(_application.EntityPath, directoryInfo.Name),
            settings.SettingsDictionary);
        await _context.SaveChangesAsync();
        return server.Id;
    }

    //TODO CKE add a lock to the server so it is locked while starting etc.
    public async Task StartEntityAsync(IEntity entity)
    {
        if (entity is not Server server)
        {
            await _console.WriteError(entity,
                $"Oops something went wrong while starting this server. More about the issue is in the logs");
            throw new ForkException($"This service can only handle servers. Supplied type was {entity.GetType()}");
        }

        await ChangeEntityStatusAsync(server, EntityStatus.Starting);
        _logger.LogInformation($"Starting server {server.Name} on world {server.VanillaSettings.LevelName}");

        // Get server directory
        DirectoryInfo serverDirectory = new DirectoryInfo(server.GetPath(_application));
        if (!serverDirectory.Exists)
        {
            await _console.WriteError(entity,
                $"This server has no directory for some reason. The path that was searched was:\n{server.GetPath(_application)}");
            throw new ForkException($"Supplied server \"{server.Name}\" has no directory!");
        }

        JavaVersion javaVersion = JavaVersionUtils.GetInstalledJavaVersion(server.JavaSettings.JavaPath);
        if (javaVersion == null)
        {
            await _console.WriteError(entity,
                $"No valid Java installation was found for the configured java path:\n{server.JavaSettings.JavaPath}");
            throw new ForkException(
                $"No valid Java installation was found for the configured java path:\n{server.JavaSettings.JavaPath}");
        }

        if (!javaVersion.Is64Bit)
        {
            await _console.WriteWarning(entity,
                "The Java installation selected for this server is a 32-bit version, which can cause errors and limits the RAM usage to 2GB");
        }

        if (javaVersion.VersionComputed < 16)
        {
            if (new ServerVersion { Version = "1.17" }.CompareTo(server.Version) <= 0)
            {
                await _console.WriteError(entity,
                    "The Java installation selected for this server is outdated. Please update Java to version 16 or higher.");
                throw new ForkException(
                    "The Java installation selected for this server is outdated. Please update Java to version 16 or higher.");
            }

            await _console.WriteWarning(entity,
                "WARN: The Java installation selected for this server is outdated. Please update Java to version 16 or higher.");
        }

        if (server.VanillaSettings.ResourcePack != "" && server.AutoSetSha1)
        {
            await UpdateResourcePackHash(server);
        }

        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            FileName = server.JavaSettings.JavaPath ?? "java",
            WorkingDirectory = serverDirectory.FullName,
            Arguments = "-Xmx" + server.JavaSettings.MaxRam + "m " +
                        server.JavaSettings.StartupParameters + " -jar server.jar nogui",
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        };
        process.StartInfo = startInfo;
        process.Start();
        // TODO CKE Task.Run(() => { viewModel.TrackPerformance(process); });
        await _console.BindProcessToConsole(server, process.StandardOutput, process.StandardError, this);
        server.ConsoleHandler = delegate(string line)
        {
            _notificationCenter.BroadcastNotification(
                new ConsoleAddNotification
                {
                    EntityId = server.Id, NewConsoleMessage = new ConsoleMessage(line, ConsoleMessageType.UserInput)
                });
            process.StandardInput.WriteLineAsync(line);
        };

        //TODO CKE add server automation
        //ServerAutomationManager.Instance.UpdateAutomation(viewModel);

        _ = Task.Run(async () =>
        {
            //TODO CKE start performance tracker here
            // var worker = new QueryStatsWorker(viewModel);

            await process.WaitForExitAsync();
            //TODO determine crash and normal stop
            await ChangeEntityStatusAsync(server, EntityStatus.Stopped);

            //TODO CKE stop performance tracking here
            // worker.Dispose();
            //TODO CKE stop automation here
            //ServerAutomationManager.Instance.UpdateAutomation(viewModel);
        });
        _logger.LogInformation("Started server " + server.Name);

        //Register new world if created
        _ = Task.Run(async () =>
        {
            while (server.Status == EntityStatus.Starting)
            {
                await Task.Delay(500);
            }

            if (server.Status == EntityStatus.Started)
            {
                // TODO CKE update Worlds as a new one might have been created
                // viewModel.InitializeWorldsList();
            }
        });
    }

    public Task StopEntityAsync(IEntity entity)
    {
        // TODO CKE
        throw new System.NotImplementedException();
    }

    public Task RestartEntityAsync(IEntity entity)
    {
        // TODO CKE
        throw new System.NotImplementedException();
    }

    public async Task ChangeEntityStatusAsync(IEntity entity, EntityStatus newStatus)
    {
        entity.Status = newStatus;
        await _notificationCenter.BroadcastNotification(new EntityStatusChangedNotification
            { EntityId = entity.Id, NewEntityStatus = newStatus });
    }

    private string RefineName(string rawName)
    {
        string result = rawName.Trim();

        string resultRaw = result;
        int i = 1;
        while (_context.ServerSet.Any(s => s.Name == result))
        {
            result = resultRaw + "(" + i + ")";
            i++;
        }

        return result;
    }

    private async Task UpdateResourcePackHash(Server server)
    {
        await _console.WriteLine(server, "Generating Resource Pack hash...");
        string resourcePackUrl = server.VanillaSettings.ResourcePack.Replace("\\", "");
        bool isHashUpToDate = await IsHashUpToDate(server.ResourcePackHashAge, resourcePackUrl, server);
        if (!string.IsNullOrEmpty(server.VanillaSettings.ResourcePackSha1) && isHashUpToDate)
        {
            await _console.WriteLine(server, "Resource Pack hash is still up to date. Staring server...");
            return;
        }

        await _console.WriteLine(server, "Resource Pack hash is outdated. Updating it...");
        DateTime hashAge = DateTime.Now;
        IProgress<float> downloadProgress = new Progress<float>();
        string hash = await HashResourcePack(resourcePackUrl, downloadProgress, server);
        if (!string.IsNullOrEmpty(hash))
        {
            server.VanillaSettings.ResourcePackSha1 = hash;
            server.ResourcePackHashAge = hashAge;
            //TODO CKE
            //await viewModel.SaveProperties();
            await _console.WriteSuccess(server, "Successfully updated Resource Pack hash to: " + hash);
            await _console.WriteLine(server, "Starting the server...");
        }
        else
        {
            await _console.WriteError(server, "Error updating the Resource Pack hash! Continuing with no hash...");
        }
    }

    private async Task<bool> IsHashUpToDate(DateTime hashDate, string fileSourceUrl, Server server)
    {
        if (string.IsNullOrEmpty(fileSourceUrl))
        {
            return false;
        }

        var client = new HttpClient();
        HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, fileSourceUrl));
        var lastModifiedString = response.Headers.GetValues("LastModified").FirstOrDefault();
        if (lastModifiedString == null)
        {
            await _console.WriteError(server,
                "Checking if resource-pack hash is up to date failed due to missing LastModified header. Updating it anyway...");
            return false;
        }

        bool canBeParsed = DateTime.TryParse(lastModifiedString, out var lastModified);

        if (canBeParsed && lastModified.CompareTo(hashDate) < 0)
        {
            return true;
        }

        return false;
    }

    private async Task<string> HashResourcePack(string url, IProgress<float> downloadProgress, Server server)
    {
        string result = "";
        if (string.IsNullOrEmpty(url))
        {
            return result;
        }

        //ensure tmp directory
        new DirectoryInfo(Path.Combine(_application.AppPath, "tmp")).Create();
        FileInfo resourcePackFile = new FileInfo(
            Path.Combine(_application.AppPath, "tmp", Guid.NewGuid().ToString()
                .Replace("-", "") + ".zip"));

        //Download the resource pack
        var client = new HttpClient();
        HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
        if (response.Headers.GetValues("ContentType").All(h => h != "application/zip"))
        {
            await _console.WriteError(server,
                "Failed to generate resource-pack hash: No zip at URL\nResuming with no hash...");
            return result;
        }

        await _download.DownloadFileAsync(url, resourcePackFile.FullName, downloadProgress, CancellationToken.None);

        //Calculate sha-1
        await using (FileStream fs = resourcePackFile.OpenRead())
        {
            await using var bs = new BufferedStream(fs);
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                byte[] hash = await sha1.ComputeHashAsync(bs);
                StringBuilder formatted = new StringBuilder(2 * hash.Length);
                foreach (var b in hash)
                {
                    formatted.Append($"{b:X2}");
                }

                result = formatted.ToString();
            }
        }

        resourcePackFile.Delete();
        return result;
    }
}