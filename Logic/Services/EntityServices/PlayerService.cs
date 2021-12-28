using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectAvery.Adapters.Mojang;
using ProjectAvery.Logic.Managers;
using ProjectAvery.Logic.Persistence;
using ProjectAvery.Logic.Services.WebServices;
using ProjectAveryCommon.ExtensionMethods;
using ProjectAveryCommon.Model.Application.Exceptions;
using ProjectAveryCommon.Model.Entity.Pocos.Player;

namespace ProjectAvery.Logic.Services.EntityServices;

public class PlayerService : IPlayerService
{
    private readonly ILogger<PlayerService> _logger;
    private readonly IObjectCache _objectCache;
    private readonly IMojangApiAdapter _mojangApi;
    
    private Dictionary<string, Player> PlayersByUid => _objectCache.PlayersByUid;

    public PlayerService(ILogger<PlayerService> logger, IObjectCache objectCache, IMojangApiAdapter mojangApi)
    {
        _logger = logger;
        _objectCache = objectCache;
        _mojangApi = mojangApi;
    }

    public async Task<Player> PlayerByNameAsync(string name)
    {
        var cachedPlayer = PlayersByUid.Values.FirstOrDefault(p => p.Name == name);
        if (cachedPlayer != null && !cachedPlayer.LastUpdated.IsOlderThan(TimeSpan.FromHours(24)))
        {
            return cachedPlayer;
        }
        
        try
        {
            string uid = await _mojangApi.UidForNameAsync(name);
            if (uid != null)
            {
                return await PlayerByUidAsync(uid);
            }
        }
        catch (MojangServiceException e)
        {
            _logger.LogError(e, "Failed to get player by name");
            return null;
        }
        
        // If the player is not in the Mojang API we handle him like an offline player
        return new Player { Name = name, Head = "TODO", LastUpdated = DateTime.Now, IsOfflinePlayer = true };
    }

    public async Task<Player> PlayerByUidAsync(string uid)
    {
        
        if (PlayersByUid.ContainsKey(uid) && !PlayersByUid[uid].LastUpdated.IsOlderThan(TimeSpan.FromHours(24)))
        {
            return PlayersByUid[uid];
        }
        
        try
        {
            var playerProfile = await _mojangApi.ProfileForUidAsync(uid);
            if (playerProfile != null)
            {
                var result = new Player
                {
                    Name = playerProfile.Name, 
                    Uid = playerProfile.Id, 
                    Head = await _mojangApi.Base64HeadFromTextureProperty(playerProfile.Properties.Where(p => p.Name == "textures").Select(p => p.Value).FirstOrDefault()), 
                    LastUpdated = DateTime.Now,
                    IsOfflinePlayer = false
                };
                if (PlayersByUid.ContainsKey(uid))
                {
                    // TODO CKE Add these changes to the DB!!
                    PlayersByUid[uid].Head = result.Head;
                    PlayersByUid[uid].Name = result.Name;
                    PlayersByUid[uid].IsOfflinePlayer = false;
                    PlayersByUid[uid].LastUpdated = DateTime.Now;
                }
                else
                {
                    PlayersByUid.Add(uid, result);
                }

                return result;
            }
        }
        catch (MojangServiceException e)
        {
            _logger.LogError(e, "Failed to get player by name");
            return null;
        }

        // If the player is not in the Mojang API we handle him like an offline player
        return new Player { Uid = uid, Head = "TODO", LastUpdated = DateTime.Now, IsOfflinePlayer = true };
    }

    public async Task<ISet<string>> PlayerUidsForWorldsAsync(List<string> worldPaths)
    {
        HashSet<string> result = new HashSet<string>();
        foreach (string worldPath in worldPaths)
        {
            DirectoryInfo world = new DirectoryInfo(worldPath);
            if (!world.Exists) continue;
            DirectoryInfo playerData = new DirectoryInfo(Path.Combine(world.FullName, "playerdata"));
            if (!playerData.Exists) continue;
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