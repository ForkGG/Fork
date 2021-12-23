using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAvery.Logic.Services.FileServices;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.Player;
using ProjectAveryCommon.Model.Entity.Pocos.ServerSettings;

namespace ProjectAvery.Logic.Services.EntityServices;

public class EntityPostProcessingService : IEntityPostProcessingService
{
    private readonly ILogger<EntityPostProcessingService> _logger;
    private readonly IFileReaderService _fileReader;
    private readonly IApplicationManager _application;
    private readonly IPlayerService _playerService;

    public EntityPostProcessingService(ILogger<EntityPostProcessingService> logger, IFileReaderService fileReader,
        IApplicationManager application, IPlayerService playerService)
    {
        _logger = logger;
        _fileReader = fileReader;
        _application = application;
        _playerService = playerService;
    }

    public async Task PostProcessEntity(IEntity entity)
    {
        await DoEntityPostProcessing(entity);
        
        if (entity is Server server)
        {
            await DoServerPostProcessing(server);
        }
    }

    private async Task DoEntityPostProcessing(IEntity entity)
    {
        await Task.CompletedTask;
    }

    private async Task DoServerPostProcessing(Server server)
    {
        string serverPath = Path.Combine(_application.EntityPath, server.Name);
        
        // Read settings files
        server.VanillaSettings =
            new VanillaSettings(
                await _fileReader.ReadVanillaSettingsAsync(serverPath));
        server.ServerPlayers ??= new List<ServerPlayer>();
        
        // Update changed players by comparing database with files
        // TODO extend this if multiple worlds are possible
        List<string> worlds = new List<string>{Path.Combine(serverPath, server.VanillaSettings.LevelName)};
        var playersInWorlds = await UidsToPlayersAsync(await _playerService.PlayerUidsForWorldsAsync(worlds));
        foreach (Player player in playersInWorlds)
        {
            var serverPlayer = server.ServerPlayers.FirstOrDefault(s => s.Player.Uid == player.Uid);
            if (serverPlayer != null)
            {
                serverPlayer.Player.Name = player.Name;
                serverPlayer.Player.Head = player.Head;
                serverPlayer.Player.LastUpdated = player.LastUpdated;
                serverPlayer.Player.IsOfflinePlayer = player.IsOfflinePlayer;
            }
            else
            {
                server.ServerPlayers.Add(new ServerPlayer{Player = player, Server = server, ServerId = server.Id});
            }
        }

        // Read whitelist, banlist and oplist
        // Version 1.7.5 and earlier have txt files
        if (server.Version.CompareTo(new ServerVersion { Version = "1.7.5" }) <= 0)
        {
            var whitelistNames = await _fileReader.ReadWhiteListTxt(serverPath);
            server.Whitelist = await NamesToPlayersAsync(whitelistNames);
            var banListNames = await _fileReader.ReadBanListTxt(serverPath);
            server.Banlist = await NamesToPlayersAsync(banListNames);
            var opListNames = await _fileReader.ReadOpListTxt(serverPath);
            foreach (string opName in opListNames)
            {
                var serverPlayer = server.ServerPlayers.FirstOrDefault(p => p.Player.Name == opName);
                if (serverPlayer != null)
                {
                    serverPlayer.IsOp = true;
                }
            }
        }
        else
        {
            var whitelistUids = await _fileReader.ReadWhiteListJson(serverPath);
            server.Whitelist = await UidsToPlayersAsync(whitelistUids);
            var banListUids = await _fileReader.ReadBanListJson(serverPath);
            server.Banlist = await UidsToPlayersAsync(banListUids);
            var opListUids = await _fileReader.ReadOpListJson(serverPath);
            foreach (string opUid in opListUids)
            {
                var serverPlayer = server.ServerPlayers.FirstOrDefault(p => p.Player.Uid == opUid);
                if (serverPlayer != null)
                {
                    serverPlayer.IsOp = true;
                }
            }
        }
    }

    private async Task<List<Player>> NamesToPlayersAsync(IEnumerable<string> names)
    {
        var result = new List<Player>();

        foreach (string name in names)
        {
            result.Add(await _playerService.PlayerByNameAsync(name));
        }
        
        return result;
    }

    private async Task<List<Player>> UidsToPlayersAsync(IEnumerable<string> uids)
    {
        var result = new List<Player>();

        foreach (string uid in uids)
        {
            result.Add(await _playerService.PlayerByUidAsync(uid));
        }

        return result;
    }
}