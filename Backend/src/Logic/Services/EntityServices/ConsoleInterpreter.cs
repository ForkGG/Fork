using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Entity.Enums.Player;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Services.EntityServices;

public class ConsoleInterpreter
{
    private const string BASE = @"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\] \[.*\]: ";
    private const string PLAYER = @"([0-9A-Za-z_]+)";
    private readonly Regex _banlistAddOldRegex = new(BASE + @"Banned player " + PLAYER + @"$");
    private readonly Regex _banlistAddRegex = new(BASE + @"Banned " + PLAYER + @": .*\.$");
    private readonly Regex _banlistRemoveRegex = new(BASE + @"Unbanned (?:player )?" + PLAYER + @"$");

    private readonly EntityManager _entityManager;

    // TODO CKE check if this works for spigot and paper
    private readonly Regex _joinRegex = new(BASE + PLAYER + @" joined the game$");
    private readonly Regex _leaveRegex = new(BASE + PLAYER + @" left the game$");


    private readonly ILogger<ConsoleInterpreter> _logger;
    private readonly Regex _opsAddOldRegex = new(BASE + @"Opped " + PLAYER + @"$");
    private readonly Regex _opsAddRegex = new(BASE + @"Made " + PLAYER + @" a server operator$");
    private readonly Regex _opsRemoveOldRegex = new(BASE + @"De-opped " + PLAYER + @"$");
    private readonly Regex _opsRemoveRegex = new(BASE + @"Made " + PLAYER + @" no longer a server operator$");
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Regex _whitelistAddRegex = new(BASE + @"Added " + PLAYER + @" to the whitelist$");
    private readonly Regex _whitelistRemoveRegex = new(BASE + @"Removed " + PLAYER + @" from the whitelist$");

    public ConsoleInterpreter(ILogger<ConsoleInterpreter> logger, EntityManager entityManager,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _entityManager = entityManager;
        _serviceScopeFactory = serviceScopeFactory;
    }


    public async Task InterpretLine(IEntity entity, string line)
    {
        if (entity is Server server)
        {
            await HandlePlayerJoinLeave(server, line);
            await HandlePlayerOps(server, line);
            await HandlePlayerWhitelist(server, line);
            await HandlePlayerBanList(server, line);
        }
    }

    private async Task HandlePlayerJoinLeave(Server server, string line)
    {
        Match joinMatch = _joinRegex.Match(line);
        if (joinMatch.Success)
        {
            string name = joinMatch.Groups[1].Value;
            Player player = await PlayerByNameAsync(name);
            _logger.LogDebug($"Player {name} joined the server {server.Name}");
            ServerPlayer serverPlayer = server.ServerPlayers?.FirstOrDefault(sp => sp.Player.Uid == player.Uid) ??
                                        new ServerPlayer(player, server);
            serverPlayer.IsOnline = true;
            await _entityManager.UpdatePlayerOnPlayerList(server, serverPlayer);
        }

        Match leaveMatch = _leaveRegex.Match(line);
        if (leaveMatch.Success)
        {
            string name = leaveMatch.Groups[1].Value;
            Player player = await PlayerByNameAsync(name);
            _logger.LogDebug($"Player {name} left the server {server.Name}");
            ServerPlayer serverPlayer = server.ServerPlayers?.FirstOrDefault(sp => sp.Player.Uid == player.Uid) ??
                                        new ServerPlayer(player, server);
            serverPlayer.IsOnline = false;
            await _entityManager.UpdatePlayerOnPlayerList(server, serverPlayer);
        }
    }

    private async Task HandlePlayerOps(Server server, string line)
    {
        Regex addRegex = server.Version?.IsEqualOrGreaterThan(ServerVersion.Version1_13) == true
            ? _opsAddRegex
            : _opsAddOldRegex;
        Match opsAddMatch = addRegex.Match(line);
        if (opsAddMatch.Success)
        {
            string name = opsAddMatch.Groups[1].Value;
            Player player = await PlayerByNameAsync(name);
            _logger.LogDebug($"Player {name} opped on server {server.Name}");
            ServerPlayer serverPlayer = server.ServerPlayers?.FirstOrDefault(sp => sp.Player.Uid == player.Uid) ??
                                        new ServerPlayer(player, server);
            serverPlayer.IsOp = true;
            await _entityManager.UpdatePlayerOnPlayerList(server, serverPlayer);
        }

        Regex removeRegex = server.Version?.IsEqualOrGreaterThan(ServerVersion.Version1_13) == true
            ? _opsRemoveRegex
            : _opsRemoveOldRegex;
        Match opsRemoveMatch = removeRegex.Match(line);
        if (opsRemoveMatch.Success)
        {
            string name = opsRemoveMatch.Groups[1].Value;
            Player player = await PlayerByNameAsync(name);
            _logger.LogDebug($"Player {name} de-opped on server {server.Name}");
            ServerPlayer serverPlayer = server.ServerPlayers?.FirstOrDefault(sp => sp.Player.Uid == player.Uid) ??
                                        new ServerPlayer(player, server);
            serverPlayer.IsOp = false;
            await _entityManager.UpdatePlayerOnPlayerList(server, serverPlayer);
        }
    }

    private async Task HandlePlayerWhitelist(Server server, string line)
    {
        Match whitelistAddMatch = _whitelistAddRegex.Match(line);
        if (whitelistAddMatch.Success)
        {
            string name = whitelistAddMatch.Groups[1].Value;
            Player player = await PlayerByNameAsync(name);
            _logger.LogDebug($"Player {name} added to the servers {server.Name} whitelist");
            await _entityManager.UpdatePlayerOnWhitelist(server, player, PlayerlistUpdateType.Add);
        }

        Match whitelistRemoveMatch = _whitelistRemoveRegex.Match(line);
        if (whitelistRemoveMatch.Success)
        {
            string name = whitelistRemoveMatch.Groups[1].Value;
            Player player = await PlayerByNameAsync(name);
            _logger.LogDebug($"Player {name} removed from the servers {server.Name} whitelist");
            await _entityManager.UpdatePlayerOnWhitelist(server, player, PlayerlistUpdateType.Remove);
        }
    }

    private async Task HandlePlayerBanList(Server server, string line)
    {
        Regex addRegex = server.Version?.IsEqualOrGreaterThan(ServerVersion.Version1_13) == true
            ? _banlistAddRegex
            : _banlistAddOldRegex;
        Match banlistAddMatch = addRegex.Match(line);
        if (banlistAddMatch.Success)
        {
            string name = banlistAddMatch.Groups[1].Value;
            Player player = await PlayerByNameAsync(name);
            _logger.LogDebug($"Player {name} banned on server {server.Name}");
            await _entityManager.UpdatePlayerOnBanList(server, player, PlayerlistUpdateType.Add);
        }

        Match banlistRemoveMatch = _banlistRemoveRegex.Match(line);
        if (banlistRemoveMatch.Success)
        {
            string name = banlistRemoveMatch.Groups[1].Value;
            Player player = await PlayerByNameAsync(name);
            _logger.LogDebug($"Player {name} unbanned on server {server.Name}");
            await _entityManager.UpdatePlayerOnBanList(server, player, PlayerlistUpdateType.Remove);
        }
    }

    /// <summary>
    ///     Gets a player object by the name of the player
    /// </summary>
    private async Task<Player> PlayerByNameAsync(string name)
    {
        // We need to create a new DI scope here, because we are in a background thread outside any request
        // If we don't do this here, the DBContext in the PlayerService will be disposed by the time we call it
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        PlayerService playerService = scope.ServiceProvider.GetRequiredService<PlayerService>();
        return await playerService.PlayerByNameAsync(name);
    }
}