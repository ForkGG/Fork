using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using Microsoft.Extensions.Logging;

namespace Fork.Adapters.Waterfall;

public class WaterfallApiAdapter(ILogger<WaterfallApiAdapter> logger, ApplicationManager applicationManager)
    : AbstractAdapter(logger, applicationManager)
{
    public async Task<List<ServerVersion>> LoadWaterfallServerVersions()
    {
        Uri uri = new("https://api.papermc.io/v2/projects/waterfall");
        WaterfallVersionList versionList = await GetAsync<WaterfallVersionList>(uri);

        return versionList.versions
            .Select(version => new ServerVersion
            {
                Type = VersionType.Waterfall,
                Version = version
            }).Reverse().ToList();
    }

    public async Task<string> GetDownloadUrlForWaterfallServerVersion(ServerVersion serverVersion)
    {
        Assert.IsTrue(serverVersion.Type == VersionType.Waterfall, "Only waterfall versions are supported!");

        string? version = serverVersion.Version;
        Assert.NotNull(version);

        string url = "https://api.papermc.io/v2/projects/waterfall/versions/" + version;
        WaterfallVersion waterfallVersion = await GetAsync<WaterfallVersion>(url);
        int latestBuild = waterfallVersion.builds.Last();

        return
            $"https://api.papermc.io/v2/projects/waterfall/versions/{version}/builds/{latestBuild}/downloads/waterfall-{version}-{latestBuild}.jar";
    }
}
