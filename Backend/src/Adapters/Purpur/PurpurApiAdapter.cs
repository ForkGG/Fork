using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using Microsoft.Extensions.Logging;

namespace Fork.Adapters.Purpur;

public class PurpurApiAdapter(ILogger<PurpurApiAdapter> logger, ApplicationManager applicationManager)
    : AbstractAdapter(logger, applicationManager)
{
    public async Task<List<ServerVersion>> LoadPurpurServerVersions()
    {
        Uri uri = new("https://api.purpurmc.org/v2/purpur/");
        PurpurVersionList versionList = await GetAsync<PurpurVersionList>(uri);

        return versionList.versions
            .Select(version => new ServerVersion
            {
                Type = VersionType.Purpur,
                Version = version
            }).Reverse().ToList();
    }

    public async Task<string> GetDownloadUrlForPurpurServerVersion(ServerVersion serverVersion)
    {
        Assert.IsTrue(serverVersion.Type == VersionType.Purpur, "Only purpur versions are supported!");

        string? version = serverVersion.Version;
        Assert.NotNull(version);

        return $"https://api.purpurmc.org/v2/purpur/{version}/latest/download";
    }
}
