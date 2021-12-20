using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Services.WebServices;

public interface IDownloadService
{
    public Task DownloadJarAsync(IEntity entity, IProgress<float> progress, CancellationToken cancellationToken);

    public Task DownloadFileAsync(string url, string destinationPath, IProgress<float> progress, CancellationToken cancellationToken);
}