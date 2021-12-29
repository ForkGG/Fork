﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Fork.Logic.Managers;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Logic.Services.WebServices;

public class DownloadService : IDownloadService
{
    private readonly ILogger<DownloadService> _logger;
    private readonly IApplicationManager _application;

    public DownloadService(ILogger<DownloadService> logger, IApplicationManager application)
    {
        _logger = logger;
        _application = application;
    }

    public async Task DownloadJarAsync(IEntity entity, IProgress<float> progress, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5);
        await using var fileStream = new FileStream(Path.Combine(_application.EntityPath, entity.Name, "server.jar"),
            FileMode.Create, FileAccess.Write, FileShare.None);
        await client.DownloadAsync(entity.Version.JarLink, fileStream, progress, cancellationToken);
    }

    public async Task DownloadFileAsync(string url, string targetPath, IProgress<float> progress, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromMinutes(5);
        await using var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await client.DownloadAsync(url, fileStream, progress, cancellationToken);
    }
}