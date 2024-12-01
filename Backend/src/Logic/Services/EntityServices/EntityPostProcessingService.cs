using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using Fork.Logic.Services.FileServices;
using Fork.Util.ExtensionMethods;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Entity.Pocos.ServerSettings;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Services.EntityServices;

public class EntityPostProcessingService
{
    private readonly ApplicationManager _application;
    private readonly FileReaderService _fileReader;
    private readonly ILogger<EntityPostProcessingService> _logger;
    private readonly PlayerService _playerService;

    public EntityPostProcessingService(ILogger<EntityPostProcessingService> logger, FileReaderService fileReader,
        ApplicationManager application, PlayerService playerService)
    {
        _logger = logger;
        _fileReader = fileReader;
        _application = application;
        _playerService = playerService;
    }

    public async Task PostProcessEntity(IEntity? entity)
    {
        await DoEntityPostProcessing(entity);

        if (entity is Server server)
        {
            await DoServerPostProcessing(server);
        }
    }

    private async Task DoEntityPostProcessing(IEntity? entity)
    {
        await Task.CompletedTask;
    }

    private async Task DoServerPostProcessing(Server server)
    {
        string serverPath = server.GetPath(_application);

        // Read settings files
        server.VanillaSettings =
            new VanillaSettings(
                await _fileReader.ReadVanillaSettingsAsync(serverPath));

        // Update changed players by comparing database with files
        // TODO extend this if multiple worlds are possible
        List<string> worlds = new() { Path.Combine(serverPath, server.VanillaSettings.LevelName) };
        List<Player> playersInWorlds = await UidsToPlayersAsync(await _playerService.PlayerUidsForWorldsAsync(worlds));
        foreach (Player player in playersInWorlds)
        {
            ServerPlayer? serverPlayer = server.ServerPlayers.FirstOrDefault(s => s.Player.Uid == player.Uid);
            if (serverPlayer != null)
            {
                serverPlayer.Player.Name = player.Name;
                serverPlayer.Player.Head = player.Head;
                serverPlayer.Player.LastUpdated = player.LastUpdated;
                serverPlayer.Player.IsOfflinePlayer = player.IsOfflinePlayer;
            }
            else
            {
                server.ServerPlayers.Add(new ServerPlayer(player, server));
            }
        }

        // Read whitelist, banlist and oplist
        // Version 1.7.5 and earlier have txt files
        if (server.Version?.CompareTo(new ServerVersion { Version = "1.7.5" }) <= 0)
        {
            List<string> whitelistNames = await _fileReader.ReadWhiteListTxt(serverPath);
            server.Whitelist = await NamesToPlayersAsync(whitelistNames);
            List<string> banListNames = await _fileReader.ReadBanListTxt(serverPath);
            server.Banlist = await NamesToPlayersAsync(banListNames);
            List<string> opListNames = await _fileReader.ReadOpListTxt(serverPath);
            foreach (string opName in opListNames)
            {
                ServerPlayer? serverPlayer = server.ServerPlayers.FirstOrDefault(p => p.Player.Name == opName);
                if (serverPlayer != null)
                {
                    serverPlayer.IsOp = true;
                }
            }
        }
        else
        {
            List<string> whitelistUids = await _fileReader.ReadWhiteListJson(serverPath);
            server.Whitelist = await UidsToPlayersAsync(whitelistUids);
            List<string> banListUids = await _fileReader.ReadBanListJson(serverPath);
            server.Banlist = await UidsToPlayersAsync(banListUids);
            List<string> opListUids = await _fileReader.ReadOpListJson(serverPath);
            foreach (string opUid in opListUids)
            {
                ServerPlayer? serverPlayer =
                    server.ServerPlayers.FirstOrDefault(p => p.Player.Uid == opUid.Replace("-", ""));
                if (serverPlayer != null)
                {
                    serverPlayer.IsOp = true;
                }
            }
        }
    }

    private async Task<List<Player>> NamesToPlayersAsync(IEnumerable<string> names)
    {
        List<Player> result = new();

        foreach (string name in names) result.Add(await _playerService.PlayerByNameAsync(name));

        return result;
    }

    private async Task<List<Player>> UidsToPlayersAsync(IEnumerable<string> uids)
    {
        List<Player> result = new();

        foreach (string uid in uids) result.Add(await _playerService.PlayerByUidAsync(uid));

        return result;
    }
}