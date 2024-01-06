using System;
using System.Threading;
using System.Threading.Tasks;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Logic.Services.WebServices;

public interface IDownloadService
{
    public Task DownloadJarAsync(IEntity entity, IProgress<float> progress, CancellationToken cancellationToken);

    public Task DownloadFileAsync(string url, string destinationPath, IProgress<float> progress,
        CancellationToken cancellationToken);
}