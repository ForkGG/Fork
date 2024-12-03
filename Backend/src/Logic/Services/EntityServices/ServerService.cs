using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using ForkCommon.Model.Notifications.EntityNotifications;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Services.EntityServices;

public class ServerService(
    ILogger<ServerService> logger,
    ApplicationDbContext context,
    ApplicationManager application,
    ConsoleService console,
    DownloadService download,
    FileWriterService fileWriter,
    NotificationCenter notificationCenter,
    ServerVersionManager serverVersionManager)
{
    public async Task<ulong> CreateServerAsync(string serverName, ServerVersion serverVersion, VanillaSettings settings,
        JavaSettings javaSettings, string? worldPath)
    {
        serverName = RefineName(serverName);
        string serverPath = Path.Combine(application.EntityPath, serverName);
        if (string.IsNullOrEmpty(settings.LevelName))
        {
            settings.LevelName = "world";
        }

        ServerVersion versionForDownload = await serverVersionManager.PrepareServerVersionForDownload(serverVersion);

        DirectoryInfo directoryInfo = Directory.CreateDirectory(serverPath);
        Server server = new(serverName, versionForDownload, settings, javaSettings);
        context.ServerSet.Add(server);

        //Download server.jar
        Progress<float> downloadProgress = new();
        downloadProgress.ProgressChanged += (_, f) =>
        {
            // TODO CKE send notification update
        };
        await download.DownloadJarAsync(server, downloadProgress, CancellationToken.None);

        //Move World Files
        if (worldPath != null)
        {
            //TODO CKE add world import
            //new FileImporter().DirectoryCopy(worldPath,
            //   Path.Combine(directoryInfo.FullName, server.VanillaSettings.LevelName), true);
        }

        //Writing necessary files
        await fileWriter.WriteEula(Path.Combine(application.EntityPath, directoryInfo.Name));
        await fileWriter.WriteServerSettings(Path.Combine(application.EntityPath, directoryInfo.Name),
            settings.SettingsDictionary);
        await context.SaveChangesAsync();
        return server.Id;
    }

    public async Task DeleteServerAsync(Server server)
    {
        if (server.Status != EntityStatus.Stopped)
        {
            await StopServerAsync(server);
            while (server.Status != EntityStatus.Stopped) await Task.Delay(500);
        }

        // TODO CKE cancel download if still in progress
        // if (!serverViewModel.DownloadCompleted)
        // {
        //     //Cancel download
        //     await Downloader.CancelJarDownloadAsync(serverViewModel);
        // }

        DirectoryInfo serverDirectory = new(server.GetPath(application));
        serverDirectory.Delete(true);
        context.ServerSet.Remove(server);
        await context.SaveChangesAsync();
    }

    public async Task StartServerAsync(Server server)
    {
        if (server.Status != EntityStatus.Stopped)
        {
            await console.WriteError(server, "Can't start server that was not properly stopped");
            throw new ForkException("Only stopped servers can be started");
        }

        await ChangeServerStatusAsync(server, EntityStatus.Starting);
        logger.LogInformation($"Starting server {server.Name} on world {server.VanillaSettings?.LevelName}");

        try
        {
            // Get server directory
            DirectoryInfo serverDirectory = new(server.GetPath(application));
            if (!serverDirectory.Exists)
            {
                await console.WriteError(server,
                    $"This server has no directory for some reason. The path that was searched was:\n{server.GetPath(application)}");
                throw new ForkException($"Supplied server \"{server.Name}\" has no directory!");
            }

            JavaVersion? javaVersion = JavaVersionUtils.GetInstalledJavaVersion(server.JavaSettings?.JavaPath);
            if (javaVersion == null)
            {
                await console.WriteError(server,
                    $"No valid Java installation was found for the configured java path:\n{server.JavaSettings?.JavaPath}");
                throw new ForkException(
                    $"No valid Java installation was found for the configured java path:\n{server.JavaSettings?.JavaPath}");
            }

            if (!javaVersion.Is64Bit)
            {
                await console.WriteWarning(server,
                    "The Java installation selected for this server is a 32-bit version, which can cause errors and limits the RAM usage to 2GB");
            }

            if (javaVersion.VersionComputed < 16)
            {
                if (new ServerVersion { Version = "1.17" }.CompareTo(server.Version) <= 0)
                {
                    await console.WriteError(server,
                        "The Java installation selected for this server is outdated. Please update Java to version 16 or higher.");
                    throw new ForkException(
                        "The Java installation selected for this server is outdated. Please update Java to version 16 or higher.");
                }

                await console.WriteWarning(server,
                    "WARN: The Java installation selected for this server is outdated. Please update Java to version 16 or higher.");
            }

            if (server.VanillaSettings?.ResourcePack != "" && server.AutoSetSha1)
            {
                await UpdateResourcePackHash(server);
            }

            Process process = new();
            ProcessStartInfo startInfo = new()
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                FileName = server.JavaSettings?.JavaPath ?? "java",
                WorkingDirectory = serverDirectory.FullName,
                Arguments = "-Xmx" + server.JavaSettings?.MaxRam + "m " +
                            server.JavaSettings?.StartupParameters + " -jar server.jar nogui",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            };
            process.StartInfo = startInfo;
            process.Start();
            CancellationTokenSource serverStoppedTokenSource = new();
            Task.Run(() => TrackServerPerformance(server, process, serverStoppedTokenSource));
            console.BindProcessToConsole(server, process.StandardOutput, process.StandardError,
                status => _ = ChangeServerStatusAsync(server, status));
            server.ConsoleHandler = delegate(string line)
            {
                console.WriteLine(server, line, ConsoleMessageType.UserInput);
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

            //Register new world if created
            _ = Task.Run(async () =>
            {
                while (server.Status == EntityStatus.Starting) await Task.Delay(500);

                if (server.Status == EntityStatus.Started)
                {
                    logger.LogInformation("Started server " + server.Name);

                    // TODO CKE update Worlds as a new one might have been created
                    // viewModel.InitializeWorldsList();
                }
            });
        }
        catch (Exception e)
        {
            await ChangeServerStatusAsync(server, EntityStatus.Stopped);
            ExceptionDispatchInfo.Capture(e).Throw();
        }
    }

    public async Task StopServerAsync(Server server)
    {
        if (server.Status != EntityStatus.Started)
        {
            await console.WriteError(server, "Can't stop server that was not properly started yet");
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
        while (server.Status != EntityStatus.Stopped) await Task.Delay(250);

        try
        {
            await StartServerAsync(server);
        }
        catch (Exception e)
        {
            logger.LogDebug($"Failed to start server.\n{e.Message}\nAborting...");
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
        await notificationCenter.BroadcastNotification(new EntityStatusChangedNotification(server.Id, newStatus));
    }

    private string RefineName(string rawName)
    {
        string result = rawName.Trim();

        string resultRaw = result;
        int i = 1;
        while (context.ServerSet.Any(s => s.Name == result))
        {
            result = resultRaw + "(" + i + ")";
            i++;
        }

        return result;
    }

    private async Task UpdateResourcePackHash(Server server)
    {
        Debug.Assert(server.VanillaSettings != null);

        await console.WriteLine(server, "Generating Resource Pack hash...");
        string? resourcePackUrl = server.VanillaSettings.ResourcePack.Replace("\\", "");
        bool isHashUpToDate = await IsHashUpToDate(server.ResourcePackHashAge, resourcePackUrl, server);
        if (!string.IsNullOrEmpty(server.VanillaSettings.ResourcePackSha1) && isHashUpToDate)
        {
            await console.WriteLine(server, "Resource Pack hash is still up to date. Staring server...");
            return;
        }

        await console.WriteLine(server, "Resource Pack hash is outdated. Updating it...");
        DateTime hashAge = DateTime.Now;
        IProgress<float> downloadProgress = new Progress<float>();
        string hash = await HashResourcePack(resourcePackUrl, downloadProgress, server);
        if (!string.IsNullOrEmpty(hash))
        {
            server.VanillaSettings.ResourcePackSha1 = hash;
            server.ResourcePackHashAge = hashAge;
            //TODO CKE
            //await viewModel.SaveProperties();
            await console.WriteSuccess(server, "Successfully updated Resource Pack hash to: " + hash);
            await console.WriteLine(server, "Starting the server...");
        }
        else
        {
            await console.WriteError(server, "Error updating the Resource Pack hash! Continuing with no hash...");
        }
    }

    private async Task<bool> IsHashUpToDate(DateTime? hashDate, string? fileSourceUrl, Server server)
    {
        if (string.IsNullOrEmpty(fileSourceUrl))
        {
            return false;
        }

        HttpClient client = new();
        HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, fileSourceUrl));
        string? lastModifiedString = response.Headers.GetValues("LastModified").FirstOrDefault();
        if (lastModifiedString == null)
        {
            await console.WriteError(server,
                "Checking if resource-pack hash is up to date failed due to missing LastModified header. Updating it anyway...");
            return false;
        }

        bool canBeParsed = DateTime.TryParse(lastModifiedString, out DateTime lastModified);

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
        new DirectoryInfo(Path.Combine(application.AppPath, "tmp")).Create();
        FileInfo resourcePackFile = new(
            Path.Combine(application.AppPath, "tmp", Guid.NewGuid().ToString()
                .Replace("-", "") + ".zip"));

        //Download the resource pack
        HttpClient client = new();
        HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
        if (response.Headers.GetValues("ContentType").All(h => h != "application/zip"))
        {
            await console.WriteError(server,
                "Failed to generate resource-pack hash: No zip at URL\nResuming with no hash...");
            return result;
        }

        await download.DownloadFileAsync(url, resourcePackFile.FullName, downloadProgress, CancellationToken.None);

        //Calculate sha-1
        await using (FileStream fs = resourcePackFile.OpenRead())
        {
            await using BufferedStream bs = new(fs);
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hash = await sha1.ComputeHashAsync(bs);
                StringBuilder formatted = new(2 * hash.Length);
                foreach (byte b in hash) formatted.Append($"{b:X2}");

                result = formatted.ToString();
            }
        }

        resourcePackFile.Delete();
        return result;
    }

    private async void TrackServerPerformance(Server server, Process process, CancellationTokenSource tokenSource)
    {
        while (!tokenSource.IsCancellationRequested && !process.HasExited)
            try
            {
                EntityPerformanceNotification notification = new(server.Id,
                    await process.CalculateCpuLoad(TimeSpan.FromSeconds(1)),
                    await process.CalculateMemLoad(server.JavaSettings!.MaxRam),
                    DateTime.Now - process.StartTime);
                await notificationCenter.BroadcastNotification(notification);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error while tracking performance of {server.FullName}");
            }
    }
}