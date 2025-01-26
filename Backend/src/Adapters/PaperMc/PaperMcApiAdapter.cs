using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using Microsoft.Extensions.Logging;

namespace Fork.Adapters.PaperMc;

public class PaperMcApiAdapter(ILogger<PaperMcApiAdapter> logger, ApplicationManager applicationManager)
    : AbstractAdapter(logger, applicationManager)
{
    public async Task<List<ServerVersion>> LoadPaperServerVersions()
    {
        Uri uri = new("https://api.papermc.io/v2/projects/paper");
        PaperMcVersionList versionList = await GetAsync<PaperMcVersionList>(uri);

        return versionList.versions
            .Select(version => new ServerVersion
            {
                Type = VersionType.Paper,
                Version = version
            }).Reverse().ToList();
    }

    public async Task<string> GetDownloadUrlForPaperServerVersion(ServerVersion serverVersion)
    {
        Assert.IsTrue(serverVersion.Type == VersionType.Paper, "Only paper versions are supported!");

        string? version = serverVersion.Version;
        Assert.NotNull(version);

        string url = "https://api.papermc.io/v2/projects/paper/versions/" + version;
        PaperMcVersion paperMcVersion = await GetAsync<PaperMcVersion>(url);
        int latestBuild = paperMcVersion.builds.Last();

        return
            $"https://api.papermc.io/v2/projects/paper/versions/{version}/builds/{latestBuild}/downloads/paper-{version}-{latestBuild}.jar";
    }
}
