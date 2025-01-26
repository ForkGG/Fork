using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fork.Adapters.Mojang;
using Fork.Adapters.PaperMc;
using Fork.Adapters.Waterfall;
using Fork.Adapters.Purpur;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using Microsoft.Extensions.DependencyInjection;

namespace Fork.Logic.Managers;

public class ServerVersionManager(IServiceProvider serviceProvider)
{
    private readonly Dictionary<VersionType, VersionCacheEntry> _versionCache = new();
    private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(120);
    public readonly List<VersionType> SupportedVersionTypes = new()
    {
      VersionType.Vanilla,
      VersionType.Paper,
      VersionType.Waterfall,
      VersionType.Purpur
    };

    public async Task<List<ServerVersion>> GetServerVersionsForType(VersionType versionType)
    {
        // Invalidate Chache if invalid
        if (_versionCache.ContainsKey(versionType) && _versionCache[versionType].ExpirationDate < DateTime.Now)
        {
            _versionCache.Remove(versionType);
        }

        if (!_versionCache.ContainsKey(versionType))
        {
            List<ServerVersion> serverVersions = await LoadServerVersionByType(versionType);
            AddVersionsToCache(versionType, serverVersions);
        }

        return _versionCache[versionType].ServerVersions;
    }

    private void AddVersionsToCache(VersionType type, List<ServerVersion> serverVersions)
    {
        _versionCache[type] = new VersionCacheEntry(type, DateTime.Now + CacheDuration, serverVersions);
    }

    private async Task<List<ServerVersion>> LoadServerVersionByType(VersionType versionType)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        if (versionType == VersionType.Vanilla)
        {
            MojangApiAdapter mojangApiAdapter = scope.ServiceProvider.GetRequiredService<MojangApiAdapter>();
            List<ServerVersion> serverVersions = await mojangApiAdapter.LoadVanillaVersions();

            // Vanilla Versions also contain Snapshot version! -> Add them to the cache as well
            AddVersionsToCache(VersionType.VanillaSnapshot,
                serverVersions.Where(v => v.Type == VersionType.VanillaSnapshot).ToList());
            return serverVersions.Where(v => v.Type == versionType).ToList();
        }

        if (versionType == VersionType.VanillaSnapshot)
        {
            MojangApiAdapter mojangApiAdapter = scope.ServiceProvider.GetRequiredService<MojangApiAdapter>();
            List<ServerVersion> serverVersions = await mojangApiAdapter.LoadVanillaVersions();

            // Snapshot versions also contains Vanilla version! -> Add them to the cache as well
            AddVersionsToCache(VersionType.Vanilla,
                serverVersions.Where(v => v.Type == VersionType.Vanilla).ToList());
            return serverVersions.Where(v => v.Type == versionType).ToList();
        }

        if (versionType == VersionType.Paper)
        {
            PaperMcApiAdapter paperMcApiAdapter = scope.ServiceProvider.GetRequiredService<PaperMcApiAdapter>();
            return await paperMcApiAdapter.LoadPaperServerVersions();
        }

        if (versionType == VersionType.Waterfall)
        {
            WaterfallApiAdapter waterfallMcApiAdapter = scope.ServiceProvider.GetRequiredService<WaterfallApiAdapter>();
            return await waterfallMcApiAdapter.LoadWaterfallServerVersions();
        }

        if (versionType == VersionType.Purpur)
        {
            PurpurApiAdapter waterfallMcApiAdapter = scope.ServiceProvider.GetRequiredService<PurpurApiAdapter>();
            return await waterfallMcApiAdapter.LoadPurpurServerVersions();
        }

        throw new ForkException("Versions of type " + versionType +
                                "are not currently supported! Report this to the Fork team.");
    }

    public async Task<ServerVersion> PrepareServerVersionForDownload(ServerVersion serverVersion)
    {
        serverVersion = serverVersion.Clone();

        switch (serverVersion.Type)
        {
            case VersionType.Vanilla:
            case VersionType.VanillaSnapshot:
                return await PrepareVanillaVersionForDownload(serverVersion);
            case VersionType.Paper:
                return await PreparePaperVersionForDownload(serverVersion);
            case VersionType.Waterfall:
                return await PrepareWaterfallVersionForDownload(serverVersion);
            case VersionType.Purpur:
                return await PreparePurpurVersionForDownload(serverVersion);
            default:
                throw new ProgrammingErrorException();
        }
    }

    private async Task<ServerVersion> PrepareVanillaVersionForDownload(ServerVersion serverVersion)
    {
        Assert.IsTrue(serverVersion.Type is VersionType.Vanilla or VersionType.VanillaSnapshot);
        using IServiceScope scope = serviceProvider.CreateScope();

        MojangApiAdapter mojangApiAdapter = scope.ServiceProvider.GetRequiredService<MojangApiAdapter>();
        serverVersion.JarLink = await mojangApiAdapter.GetDownloadUrlForPaperServerVersion(serverVersion);
        return serverVersion;
    }

    private async Task<ServerVersion> PreparePaperVersionForDownload(ServerVersion serverVersion)
    {
        Assert.IsTrue(serverVersion.Type == VersionType.Paper);
        using IServiceScope scope = serviceProvider.CreateScope();

        PaperMcApiAdapter paperMcApiAdapter = scope.ServiceProvider.GetRequiredService<PaperMcApiAdapter>();
        serverVersion.JarLink = await paperMcApiAdapter.GetDownloadUrlForPaperServerVersion(serverVersion);
        return serverVersion;
    }

    private async Task<ServerVersion> PrepareWaterfallVersionForDownload(ServerVersion serverVersion)
    {
        Assert.IsTrue(serverVersion.Type == VersionType.Waterfall);
        using IServiceScope scope = serviceProvider.CreateScope();

        WaterfallApiAdapter paperMcApiAdapter = scope.ServiceProvider.GetRequiredService<WaterfallApiAdapter>();
        serverVersion.JarLink = await paperMcApiAdapter.GetDownloadUrlForWaterfallServerVersion(serverVersion);
        return serverVersion;
    }

    private async Task<ServerVersion> PreparePurpurVersionForDownload(ServerVersion serverVersion)
    {
        Assert.IsTrue(serverVersion.Type == VersionType.Purpur);
        using IServiceScope scope = serviceProvider.CreateScope();

        PurpurApiAdapter paperMcApiAdapter = scope.ServiceProvider.GetRequiredService<PurpurApiAdapter>();
        serverVersion.JarLink = await paperMcApiAdapter.GetDownloadUrlForPurpurServerVersion(serverVersion);
        return serverVersion;
    }

    private record VersionCacheEntry(
        VersionType VersionType,
        DateTime ExpirationDate,
        List<ServerVersion> ServerVersions);
}
