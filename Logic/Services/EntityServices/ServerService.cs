using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Fork.Logic.Managers;
using Fork.Logic.Notification;
using Fork.Logic.Persistence;
using Fork.Logic.Services.FileServices;
using Fork.Logic.Services.WebServices;
using Fork.Util.ExtensionMethods;
using Fork.Util.JavaUtils;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Enums.Console;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.ServerSettings;
using ForkCommon.Model.Entity.Transient.Console;
using ForkCommon.Model.Notifications.EntityNotifications;

namespace Fork.Logic.Services.EntityServices;

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

    public async Task StartServerAsync(Server server)
    {
        if (server.Status != EntityStatus.Stopped)
        {
            await _console.WriteError(server, "Can't start server that was not properly stopped");
            throw new ForkException("Only stopped servers can be started");
        }
        await ChangeServerStatusAsync(server, EntityStatus.Starting);
        _logger.LogInformation($"Starting server {server.Name} on world {server.VanillaSettings.LevelName}");

        // Get server directory
        DirectoryInfo serverDirectory = new DirectoryInfo(server.GetPath(_application));
        if (!serverDirectory.Exists)
        {
            await ChangeServerStatusAsync(server, EntityStatus.Stopped);
            await _console.WriteError(server,
                $"This server has no directory for some reason. The path that was searched was:\n{server.GetPath(_application)}");
            throw new ForkException($"Supplied server \"{server.Name}\" has no directory!");
        }

        JavaVersion javaVersion = JavaVersionUtils.GetInstalledJavaVersion(server.JavaSettings.JavaPath);
        if (javaVersion == null)
        {
            await ChangeServerStatusAsync(server, EntityStatus.Stopped);
            await _console.WriteError(server,
                $"No valid Java installation was found for the configured java path:\n{server.JavaSettings.JavaPath}");
            throw new ForkException(
                $"No valid Java installation was found for the configured java path:\n{server.JavaSettings.JavaPath}");
        }

        if (!javaVersion.Is64Bit)
        {
            await _console.WriteWarning(server,
                "The Java installation selected for this server is a 32-bit version, which can cause errors and limits the RAM usage to 2GB");
        }

        if (javaVersion.VersionComputed < 16)
        {
            if (new ServerVersion { Version = "1.17" }.CompareTo(server.Version) <= 0)
            {
                await ChangeServerStatusAsync(server, EntityStatus.Stopped);
                await _console.WriteError(server,
                    "The Java installation selected for this server is outdated. Please update Java to version 16 or higher.");
                throw new ForkException(
                    "The Java installation selected for this server is outdated. Please update Java to version 16 or higher.");
            }

            await _console.WriteWarning(server,
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
        CancellationTokenSource serverStoppedTokenSource = new CancellationTokenSource();
        Task.Run(() => TrackServerPerformance(server, process, serverStoppedTokenSource));
        await _console.BindProcessToConsole(server, process.StandardOutput, process.StandardError,
            status => _ = ChangeServerStatusAsync(server, status));
        server.ConsoleHandler = delegate(string line)
        {
            _console.WriteLine(server, line, ConsoleMessageType.UserInput);
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
            await ChangeServerStatusAsync(server, EntityStatus.Stopped);

            serverStoppedTokenSource.Cancel();
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

    public async Task StopServerAsync(Server server)
    {
        if (server.Status != EntityStatus.Started)
        {
            await _console.WriteError(server, "Can't stop server that was not properly started yet");
            throw new ForkException("Only started servers can be stopped");
        }
        
        if (server.ConsoleHandler == null)
        {
            throw new ForkException("Can't stop server with no active input handler");
        }

        await ChangeServerStatusAsync(server, EntityStatus.Stopping);
        server.ConsoleHandler.Invoke("/stop");

        /*
         TODO CKE add playerlist
         foreach (ServerPlayer serverPlayer in serverViewModel.PlayerList)
        {
            serverPlayer.IsOnline = false;
        }

        serverViewModel.RefreshPlayerList();
        */
    }

    public async Task RestartServerAsync(Server server)
    {
        await StopServerAsync(server);
        while (server.Status != EntityStatus.Stopped)
        {
            await Task.Delay(250);
        }

        try
        {
            await StartServerAsync(server);
        }
        catch (Exception e)
        {
            _logger.LogDebug($"Failed to start server.\n{e.Message}\nAborting...");
            // Make sure status is stopped after starting fails
            if (server.Status != EntityStatus.Stopped)
            {
                await ChangeServerStatusAsync(server, EntityStatus.Stopped);
            }
            throw;
        }
    }

    public async Task ChangeServerStatusAsync(Server server, EntityStatus newStatus)
    {
        server.Status = newStatus;
        await _notificationCenter.BroadcastNotification(new EntityStatusChangedNotification
            { EntityId = server.Id, NewEntityStatus = newStatus });
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
            using (SHA1 sha1 = SHA1.Create())
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

    private async void TrackServerPerformance(Server server, Process process, CancellationTokenSource tokenSource)
    {
        while (!tokenSource.IsCancellationRequested && !process.HasExited)
        {
            try
            {
                var notification = new EntityPerformanceNotification
                {
                    EntityId = server.Id,
                    Uptime = DateTime.Now - process.StartTime,
                    RamPercentage = await process.CalculateMemLoad(server.JavaSettings.MaxRam),
                    CpuPercentage = await process.CalculateCpuLoad(TimeSpan.FromSeconds(1)),
                };
                await _notificationCenter.BroadcastNotification(notification);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while tracking performance of {server.FullName}");
            }
            finally
            {
                //await Task.Delay(1000);
            }
        }
    }
}