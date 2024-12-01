using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using Fork.Util.ExtensionMethods;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Pocos;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Services.WebServices;

public class DownloadService
{
    private readonly ApplicationManager _application;
    private readonly ILogger<DownloadService> _logger;

    public DownloadService(ILogger<DownloadService> logger, ApplicationManager application)
    {
        _logger = logger;
        _application = application;
    }

    public async Task DownloadJarAsync(IEntity entity, IProgress<float> progress, CancellationToken cancellationToken)
    {
        using HttpClient client = new();
        client.Timeout = TimeSpan.FromMinutes(5);
        await using FileStream fileStream = new(
            Path.Combine(entity.GetPath(_application), "server.jar"),
            FileMode.Create, FileAccess.Write, FileShare.None);
        if (entity.Version?.JarLink == null)
        {
            throw new IllegalInternalStateException("Version is missing JarLink! Version setup is wrong");
        }

        await client.DownloadAsync(entity.Version.JarLink, fileStream, progress, cancellationToken);
    }

    public async Task DownloadFileAsync(string url, string targetPath, IProgress<float> progress,
        CancellationToken cancellationToken)
    {
        using HttpClient client = new();
        client.Timeout = TimeSpan.FromMinutes(5);
        await using FileStream fileStream = new(targetPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await client.DownloadAsync(url, fileStream, progress, cancellationToken);
    }
}