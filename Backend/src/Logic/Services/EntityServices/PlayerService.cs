using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fork.Adapters.Mojang;
using Fork.Logic.Model.Web.Mojang;
using Fork.Logic.Persistence;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Pocos.Player;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Services.EntityServices;

public class PlayerService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<PlayerService> _logger;
    private readonly MojangApiAdapter _mojangApi;

    public PlayerService(ILogger<PlayerService> logger, ApplicationDbContext applicationDbContext,
        MojangApiAdapter mojangApi)
    {
        _logger = logger;
        _applicationDbContext = applicationDbContext;
        _mojangApi = mojangApi;
    }

    public async Task<Player> PlayerByNameAsync(string name)
    {
        Player? cachedPlayer = _applicationDbContext.PlayerSet.FirstOrDefault(p => p.Name == name);
        if (cachedPlayer != null && !cachedPlayer.LastUpdated.IsOlderThan(TimeSpan.FromHours(24)))
        {
            return cachedPlayer;
        }

        try
        {
            string? uid = await _mojangApi.UidForNameAsync(name);
            if (uid != null)
            {
                return await PlayerByUidAsync(uid);
            }
        }
        catch (ExternalServiceException e)
        {
            _logger.LogError(e, "Failed to get player by name");
        }

        // If the player is not in the Mojang API we handle him like an offline player
        return new Player("Offline-" + Guid.NewGuid())
            { Name = name, Head = "TODO", LastUpdated = DateTime.Now, IsOfflinePlayer = true };
    }

    public async Task<Player> PlayerByUidAsync(string uid)
    {
        uid = uid.Replace("-", "");
        Player? existingPlayer = await _applicationDbContext.PlayerSet.FirstOrDefaultAsync(p => p.Uid == uid);
        if (existingPlayer != null && !existingPlayer.LastUpdated.IsOlderThan(TimeSpan.FromHours(24)))
        {
            return existingPlayer;
        }

        try
        {
            PlayerProfile? playerProfile = await _mojangApi.ProfileForUidAsync(uid);
            if (playerProfile?.Properties != null && playerProfile.Id != null)
            {
                if (existingPlayer != null)
                {
                    existingPlayer.Head = await _mojangApi.Base64HeadFromTextureProperty(playerProfile.Properties
                        .Where(p => p.Name == "textures").Select(p => p.Value).FirstOrDefault());
                    existingPlayer.Name = playerProfile.Name;
                    existingPlayer.IsOfflinePlayer = false;
                    existingPlayer.LastUpdated = DateTime.Now;
                }
                else
                {
                    _applicationDbContext.PlayerSet.Add(new Player(playerProfile.Id)
                    {
                        Name = playerProfile.Name,
                        Head = await _mojangApi.Base64HeadFromTextureProperty(playerProfile.Properties
                            .Where(p => p.Name == "textures").Select(p => p.Value).FirstOrDefault()),
                        LastUpdated = DateTime.Now,
                        IsOfflinePlayer = false
                    });
                }

                await _applicationDbContext.SaveChangesAsync();

                Player? result = await _applicationDbContext.PlayerSet.FirstOrDefaultAsync(p => p.Uid == uid);
                if (result != null)
                {
                    return result;
                }
            }
        }
        catch (ExternalServiceException e)
        {
            _logger.LogError(e, "Failed to get player by name");
        }

        // If the player is not in the Mojang API we handle him like an offline player
        return new Player(uid) { Head = "TODO", LastUpdated = DateTime.Now, IsOfflinePlayer = true };
    }

    public async Task<ISet<string>> PlayerUidsForWorldsAsync(List<string> worldPaths)
    {
        HashSet<string> result = new();
        foreach (string worldPath in worldPaths)
        {
            DirectoryInfo world = new(worldPath);
            if (!world.Exists)
            {
                continue;
            }

            DirectoryInfo playerData = new(Path.Combine(world.FullName, "playerdata"));
            if (!playerData.Exists)
            {
                continue;
            }

            await Task.Run(() =>
            {
                foreach (string fileName in Directory.GetFiles(playerData.FullName, "*.dat",
                             SearchOption.TopDirectoryOnly))
                {
                    string uuid = new FileInfo(fileName).Name.Replace("-", "").Replace(".dat", "");
                    if (ValidateUuid(uuid))
                    {
                        result.Add(uuid);
                    }
                }
            });
        }

        return result;
    }

    // Validates a player Uuid
    private bool ValidateUuid(string uuid)
    {
        try
        {
            byte[] ba = Enumerable.Range(0, uuid.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(uuid.Substring(x, 2), 16))
                .ToArray();

            return ba.Length == 16;
        }
        catch (Exception)
        {
            return false;
        }
    }
}