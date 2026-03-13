using System.Threading.Tasks;
using FluentAssertions;
using Fork.Logic.Managers;
using Fork.Logic.Notification;
using Fork.Logic.Persistence;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Enums.Player;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Entity.Pocos.Settings;
using ForkCommon.Model.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Fork.Backend.Tests;

/// <summary>
/// Tests for the player list mutation logic in EntityManager.
/// The DB and notification side effects are handled via DI mocking.
/// </summary>
public class EntityManagerTests
{
    // ── Helpers ────────────────────────────────────────────────────────────────

    private static Server MakeServer(ulong id = 1)
    {
        var sv = new ServerVersion { Type = VersionType.Vanilla, Version = "1.20.4" };
        var server = new Server("Test", sv, new VanillaSettings("world"), new JavaSettings()) { Id = id };
        return server;
    }

    private static Player MakePlayer(string uid = "abc123", string name = "Steve") =>
        new(uid) { Name = name, LastUpdated = DateTime.Now };

    /// <summary>
    /// Creates an EntityManager whose DI scope returns an in-memory DbContext
    /// and a mock INotificationCenter.
    /// </summary>
    private static (EntityManager manager, Mock<INotificationCenter> notifMock) BuildSut()
    {
        var notifMock = new Mock<INotificationCenter>();
        notifMock
            .Setup(n => n.BroadcastNotification(It.IsAny<AbstractNotification>()))
            .Returns(Task.CompletedTask);

        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var fileReaderMock = new Mock<Fork.Logic.Services.FileServices.FileReaderService>(
            NullLogger<Fork.Logic.Services.FileServices.FileReaderService>.Instance,
            new Fork.Logic.Services.FileServices.FileWriterService());

        var dbContext = new ApplicationDbContext(
            dbOptions,
            NullLogger<ApplicationDbContext>.Instance,
            fileReaderMock.Object);

        var serviceMock = new Mock<IServiceProvider>();
        serviceMock
            .Setup(sp => sp.GetService(typeof(ApplicationDbContext)))
            .Returns(dbContext);
        serviceMock
            .Setup(sp => sp.GetService(typeof(INotificationCenter)))
            .Returns(notifMock.Object);

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.Setup(s => s.ServiceProvider).Returns(serviceMock.Object);

        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        var manager = new EntityManager(NullLogger<EntityManager>.Instance, scopeFactoryMock.Object);
        return (manager, notifMock);
    }

    // ── UpdatePlayerOnPlayerList ───────────────────────────────────────────────

    [Fact]
    public async Task UpdatePlayerOnPlayerList_NewPlayer_AddsToList()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("uid1");
        var serverPlayer = new ServerPlayer(player, server) { IsOnline = true };

        await manager.UpdatePlayerOnPlayerList(server, serverPlayer);

        server.ServerPlayers.Should().ContainSingle(sp => sp.Player.Uid == "uid1");
    }

    [Fact]
    public async Task UpdatePlayerOnPlayerList_ExistingPlayer_UpdatesStatus()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("uid1");
        var serverPlayer = new ServerPlayer(player, server) { IsOnline = true, IsOp = false };
        server.ServerPlayers.Add(serverPlayer);

        var update = new ServerPlayer(player, server) { IsOnline = false, IsOp = true };
        await manager.UpdatePlayerOnPlayerList(server, update);

        server.ServerPlayers.Should().ContainSingle();
        server.ServerPlayers[0].IsOnline.Should().BeFalse();
        server.ServerPlayers[0].IsOp.Should().BeTrue();
    }

    [Fact]
    public async Task UpdatePlayerOnPlayerList_SendsNotification()
    {
        var (manager, notifMock) = BuildSut();
        var server = MakeServer();
        var serverPlayer = new ServerPlayer(MakePlayer(), server);

        await manager.UpdatePlayerOnPlayerList(server, serverPlayer);

        notifMock.Verify(n => n.BroadcastNotification(It.IsAny<AbstractNotification>()), Times.Once);
    }

    // ── UpdatePlayerOnWhitelist ────────────────────────────────────────────────

    [Fact]
    public async Task UpdatePlayerOnWhitelist_Add_AppendsPlayer()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("uid1");

        await manager.UpdatePlayerOnWhitelist(server, player, PlayerlistUpdateType.Add);

        server.Whitelist.Should().ContainSingle(p => p.Uid == "uid1");
    }

    [Fact]
    public async Task UpdatePlayerOnWhitelist_Add_DuplicateUid_UpdatesInsteadOfDuplicating()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("uid1", "Steve");
        server.Whitelist.Add(player);

        var updated = MakePlayer("uid1", "SteveRenamed");
        await manager.UpdatePlayerOnWhitelist(server, updated, PlayerlistUpdateType.Add);

        server.Whitelist.Should().ContainSingle("duplicate should not be added");
        server.Whitelist[0].Name.Should().Be("SteveRenamed");
    }

    [Fact]
    public async Task UpdatePlayerOnWhitelist_Remove_RemovesPlayer()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("uid1");
        server.Whitelist.Add(player);

        await manager.UpdatePlayerOnWhitelist(server, player, PlayerlistUpdateType.Remove);

        server.Whitelist.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdatePlayerOnWhitelist_Remove_NonExistentPlayer_LeavesListEmpty()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();

        await manager.UpdatePlayerOnWhitelist(server, MakePlayer("uid1"), PlayerlistUpdateType.Remove);

        server.Whitelist.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdatePlayerOnWhitelist_Update_ExistingPlayer_UpdatesProperties()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var original = MakePlayer("uid1", "OldName");
        server.Whitelist.Add(original);

        var updated = MakePlayer("uid1", "NewName");
        await manager.UpdatePlayerOnWhitelist(server, updated, PlayerlistUpdateType.Update);

        server.Whitelist.Should().ContainSingle();
        server.Whitelist[0].Name.Should().Be("NewName");
    }

    [Fact]
    public async Task UpdatePlayerOnWhitelist_Update_MissingPlayer_AddsPlayer()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("uid1");

        // Update for a player not yet in the list — should add them
        await manager.UpdatePlayerOnWhitelist(server, player, PlayerlistUpdateType.Update);

        server.Whitelist.Should().ContainSingle(p => p.Uid == "uid1");
    }

    [Fact]
    public async Task UpdatePlayerOnWhitelist_SendsNotification()
    {
        var (manager, notifMock) = BuildSut();
        var server = MakeServer();

        await manager.UpdatePlayerOnWhitelist(server, MakePlayer(), PlayerlistUpdateType.Add);

        notifMock.Verify(n => n.BroadcastNotification(It.IsAny<AbstractNotification>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlayerOnWhitelist_InvalidUpdateType_Throws()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();

        Func<Task> act = () => manager.UpdatePlayerOnWhitelist(server, MakePlayer(), (PlayerlistUpdateType)99);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    // ── UpdatePlayerOnBanList ──────────────────────────────────────────────────

    [Fact]
    public async Task UpdatePlayerOnBanList_Add_AppendsBannedPlayer()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("ban1");

        await manager.UpdatePlayerOnBanList(server, player, PlayerlistUpdateType.Add);

        server.Banlist.Should().ContainSingle(p => p.Uid == "ban1");
    }

    [Fact]
    public async Task UpdatePlayerOnBanList_Add_DuplicateUid_UpdatesInsteadOfDuplicating()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("ban1", "Griefer");
        server.Banlist.Add(player);

        var updated = MakePlayer("ban1", "GrieferNewName");
        await manager.UpdatePlayerOnBanList(server, updated, PlayerlistUpdateType.Add);

        server.Banlist.Should().ContainSingle("duplicate should not be added");
        server.Banlist[0].Name.Should().Be("GrieferNewName");
    }

    [Fact]
    public async Task UpdatePlayerOnBanList_Remove_RemovesBannedPlayer()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();
        var player = MakePlayer("ban1");
        server.Banlist.Add(player);

        await manager.UpdatePlayerOnBanList(server, player, PlayerlistUpdateType.Remove);

        server.Banlist.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdatePlayerOnBanList_Remove_NonExistentPlayer_LeavesListEmpty()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();

        await manager.UpdatePlayerOnBanList(server, MakePlayer("uid1"), PlayerlistUpdateType.Remove);

        server.Banlist.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdatePlayerOnBanList_Update_MissingPlayer_AddsPlayer()
    {
        var (manager, _) = BuildSut();
        var server = MakeServer();

        await manager.UpdatePlayerOnBanList(server, MakePlayer("uid1"), PlayerlistUpdateType.Update);

        server.Banlist.Should().ContainSingle(p => p.Uid == "uid1");
    }

    [Fact]
    public async Task UpdatePlayerOnBanList_SendsNotification()
    {
        var (manager, notifMock) = BuildSut();
        var server = MakeServer();

        await manager.UpdatePlayerOnBanList(server, MakePlayer(), PlayerlistUpdateType.Add);

        notifMock.Verify(n => n.BroadcastNotification(It.IsAny<AbstractNotification>()), Times.Once);
    }
}
