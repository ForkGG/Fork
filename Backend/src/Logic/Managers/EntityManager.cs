using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fork.Logic.Notification;
using Fork.Logic.Persistence;
using Fork.Logic.Services.EntityServices;
using ForkCommon.Model.Entity.Enums.Player;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Notifications.EntityNotifications.PlayerNotifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Managers;

public class EntityManager
{
    private static readonly SemaphoreSlim Semaphore = new(1);

    // This contains all loaded entities (Database cache and state keeping)
    private readonly Dictionary<ulong, IEntity> _entities;

    private readonly ILogger<EntityManager> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public EntityManager(ILogger<EntityManager> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _entities = new Dictionary<ulong, IEntity>();
    }

    /// <summary>
    ///     Get an entity by ID and loads it form the DB if the entity is not yet loaded
    /// </summary>
    public async Task<IEntity?> EntityById(ulong entityId)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        // We need a lock here to ensure that we don't get multiple instances of the same entity
        await Semaphore.WaitAsync();
        if (_entities.ContainsKey(entityId))
        {
            Semaphore.Release();
            return _entities[entityId];
        }

        // TODO extend once we got networks
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        IEntity? result = await context.ServerSet.Where(s => s.Id == entityId)
            .Include(s => s.AutomationTimes)
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            .ThenInclude(a => a.Time)
            .Include(s => s.JavaSettings)
            .Include(s => s.Version)
            .Include(s => s.ServerPlayers)
            .ThenInclude(s => s.Player)
            .FirstOrDefaultAsync();

        if (result != null)
        {
            _entities.Add(entityId, result);
        }

        // Post processing can be done without the lock
        EntityPostProcessingService postProcessing =
            scope.ServiceProvider.GetRequiredService<EntityPostProcessingService>();
        await postProcessing.PostProcessEntity(result);
        await context.SaveChangesAsync();
        Semaphore.Release();
        return result;
    }

    public async Task<List<IEntity>> ListAllEntities()
    {
        List<IEntity> result = new();

        using IServiceScope scope = _scopeFactory.CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        foreach (ulong serverId in context.ServerSet.Select(s => s.Id))
        {
            IEntity? entity = await EntityById(serverId);
            if (entity != null)
            {
                result.Add(entity);
            }
        }

        return result;
    }

    /// <summary>
    ///     Add or update a player on the player list and save changes to DB
    /// </summary>
    public async Task UpdatePlayerOnPlayerList(Server server, ServerPlayer player)
    {
        // Player already exists -> update
        ServerPlayer? existingPlayer = server.ServerPlayers.FirstOrDefault(p => p.Player.Uid == player.Player.Uid);
        if (existingPlayer != null)
        {
            existingPlayer.IsOnline = player.IsOnline;
            existingPlayer.IsOp = player.IsOp;
        }
        // Player does not exist -> add
        else
        {
            server.ServerPlayers.Add(player);
        }

        using IServiceScope scope = _scopeFactory.CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.SaveChangesAsync();

        // Send notification
        UpdatePlayerNotification notification = new(server.Id, player);
        NotificationCenter notificationCenter = scope.ServiceProvider.GetRequiredService<NotificationCenter>();
        await notificationCenter.BroadcastNotification(notification);
    }

    public async Task UpdatePlayerOnWhitelist(Server server, Player player, PlayerlistUpdateType updateType)
    {
        switch (updateType)
        {
            case PlayerlistUpdateType.Add:
                if (server.Whitelist.Any(p => p.Uid == player.Uid))
                {
                    _logger.LogWarning(
                        "Adding player to the whitelist which is already present. Updating old entry...");
                    updateType = PlayerlistUpdateType.Update;
                    UpdatePlayerOnList(player, server.Whitelist);
                }
                else
                {
                    server.Whitelist.Add(player);
                }

                break;
            case PlayerlistUpdateType.Update:
                Player? existingPlayer = server.Whitelist.FirstOrDefault(p => p.Uid == player.Uid);
                if (existingPlayer == null)
                {
                    _logger.LogWarning("Tried to update entry on the whitelist which is not present. Adding it...");
                    updateType = PlayerlistUpdateType.Add;
                    server.Whitelist.Add(player);
                }
                else
                {
                    UpdatePlayerOnList(player, server.Whitelist);
                }

                break;
            case PlayerlistUpdateType.Remove:
                server.Whitelist.RemoveAll(p => p.Uid == player.Uid);
                break;
            default: throw new ArgumentException($"Unknown update type {updateType}");
        }

        UpdateWhitelistPlayerNotification notification = new(server.Id, updateType, player);
        using IServiceScope scope = _scopeFactory.CreateScope();
        NotificationCenter notificationCenter = scope.ServiceProvider.GetRequiredService<NotificationCenter>();
        await notificationCenter.BroadcastNotification(notification);
    }

    public async Task UpdatePlayerOnBanList(Server server, Player player, PlayerlistUpdateType updateType)
    {
        switch (updateType)
        {
            case PlayerlistUpdateType.Add:
                if (server.Banlist.Any(p => p.Uid == player.Uid))
                {
                    _logger.LogWarning("Adding player to the banlist which is already present. Updating old entry...");
                    updateType = PlayerlistUpdateType.Update;
                    UpdatePlayerOnList(player, server.Banlist);
                }
                else
                {
                    server.Banlist.Add(player);
                }

                break;
            case PlayerlistUpdateType.Update:
                Player? existingPlayer = server.Banlist.FirstOrDefault(p => p.Uid == player.Uid);
                if (existingPlayer == null)
                {
                    _logger.LogWarning("Tried to update entry on the banlist which is not present. Adding it...");
                    updateType = PlayerlistUpdateType.Add;
                    server.Banlist.Add(player);
                }
                else
                {
                    UpdatePlayerOnList(player, server.Banlist);
                }

                break;
            case PlayerlistUpdateType.Remove:
                server.Banlist.RemoveAll(p => p.Uid == player.Uid);
                break;
            default: throw new ArgumentException($"Unknown update type {updateType}");
        }

        UpdateBanlistPlayerNotification notification = new(server.Id, updateType, player);
        using IServiceScope scope = _scopeFactory.CreateScope();
        NotificationCenter notificationCenter = scope.ServiceProvider.GetRequiredService<NotificationCenter>();
        await notificationCenter.BroadcastNotification(notification);
    }

    private void UpdatePlayerOnList(Player player, List<Player> players)
    {
        Debug.Assert(players.Any(p => p.Uid == player.Uid));
        Player existingPlayer = players.First(p => p.Uid == player.Uid);
        existingPlayer.Head = player.Head;
        existingPlayer.Name = player.Name;
        existingPlayer.LastUpdated = player.LastUpdated;
        existingPlayer.IsOfflinePlayer = player.IsOfflinePlayer;
    }
}