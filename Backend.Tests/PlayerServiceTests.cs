using System.Threading.Tasks;
using FluentAssertions;
using Fork.Adapters.Mojang;
using Fork.Logic.Model.Web.Mojang;
using Fork.Logic.Persistence;
using Fork.Logic.Services.EntityServices;
using Fork.Logic.Services.FileServices;
using ForkCommon.Model.Entity.Pocos.Player;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Fork.Backend.Tests;

public class PlayerServiceTests
{
    // ── Test infrastructure ────────────────────────────────────────────────────

    private static ApplicationDbContext CreateInMemoryDb()
    {
        var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var fileReaderMock = new Mock<FileReaderService>(
            NullLogger<FileReaderService>.Instance,
            new FileWriterService());

        return new ApplicationDbContext(
            dbOptions,
            NullLogger<ApplicationDbContext>.Instance,
            fileReaderMock.Object);
    }

    private static PlayerService BuildSut(ApplicationDbContext db, Mock<IMojangApiAdapter>? mojangMock = null)
    {
        mojangMock ??= new Mock<IMojangApiAdapter>();
        return new PlayerService(NullLogger<PlayerService>.Instance, db, mojangMock.Object);
    }

    // ── PlayerByNameAsync – cache hit ──────────────────────────────────────────

    [Fact]
    public async Task PlayerByNameAsync_CachedFreshPlayer_ReturnsCachedWithoutCallingMojang()
    {
        var db = CreateInMemoryDb();
        var mojangMock = new Mock<IMojangApiAdapter>();

        var cached = new Player("abc123") { Name = "Steve", LastUpdated = DateTime.Now };
        db.PlayerSet.Add(cached);
        await db.SaveChangesAsync();

        var sut = BuildSut(db, mojangMock);
        var result = await sut.PlayerByNameAsync("Steve");

        result.Should().BeSameAs(cached);
        mojangMock.Verify(m => m.UidForNameAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PlayerByNameAsync_CachedStalePlayer_QueriesMojang()
    {
        var db = CreateInMemoryDb();
        var mojangMock = new Mock<IMojangApiAdapter>();
        mojangMock.Setup(m => m.UidForNameAsync("Steve")).ReturnsAsync("newuid");

        var stale = new Player("olduid") { Name = "Steve", LastUpdated = DateTime.Now - TimeSpan.FromHours(25) };
        db.PlayerSet.Add(stale);
        await db.SaveChangesAsync();

        var sut = BuildSut(db, mojangMock);
        await sut.PlayerByNameAsync("Steve");

        mojangMock.Verify(m => m.UidForNameAsync("Steve"), Times.Once);
    }

    // ── PlayerByNameAsync – offline fallback ───────────────────────────────────

    [Fact]
    public async Task PlayerByNameAsync_PlayerNotInMojang_ReturnsOfflinePlayer()
    {
        var db = CreateInMemoryDb();
        var mojangMock = new Mock<IMojangApiAdapter>();
        mojangMock.Setup(m => m.UidForNameAsync(It.IsAny<string>())).ReturnsAsync((string?)null);

        var sut = BuildSut(db, mojangMock);
        var result = await sut.PlayerByNameAsync("UnknownPlayer");

        result.IsOfflinePlayer.Should().BeTrue();
        result.Name.Should().Be("UnknownPlayer");
    }

    // ── PlayerByUidAsync – cache hit ───────────────────────────────────────────

    [Fact]
    public async Task PlayerByUidAsync_CachedFreshPlayer_ReturnsCachedWithoutCallingMojang()
    {
        var db = CreateInMemoryDb();
        var mojangMock = new Mock<IMojangApiAdapter>();

        var cached = new Player("abc123") { Name = "Steve", LastUpdated = DateTime.Now };
        db.PlayerSet.Add(cached);
        await db.SaveChangesAsync();

        var sut = BuildSut(db, mojangMock);
        var result = await sut.PlayerByUidAsync("abc123");

        result.Uid.Should().Be("abc123");
        mojangMock.Verify(m => m.ProfileForUidAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PlayerByUidAsync_HyphensInUid_NormalizesBeforeLookup()
    {
        var db = CreateInMemoryDb();
        var mojangMock = new Mock<IMojangApiAdapter>();

        var cached = new Player("abc123def456") { Name = "Steve", LastUpdated = DateTime.Now };
        db.PlayerSet.Add(cached);
        await db.SaveChangesAsync();

        var sut = BuildSut(db, mojangMock);
        // Provide uid with hyphens — should be stripped
        var result = await sut.PlayerByUidAsync("abc123-def456");

        result.Uid.Should().Be("abc123def456");
    }

    // ── PlayerByUidAsync – offline fallback ────────────────────────────────────

    [Fact]
    public async Task PlayerByUidAsync_NotInMojang_ReturnsOfflinePlayer()
    {
        var db = CreateInMemoryDb();
        var mojangMock = new Mock<IMojangApiAdapter>();
        mojangMock.Setup(m => m.ProfileForUidAsync(It.IsAny<string>()))
            .ReturnsAsync((PlayerProfile?)null);

        var sut = BuildSut(db, mojangMock);
        var result = await sut.PlayerByUidAsync("nonexistentuid");

        result.IsOfflinePlayer.Should().BeTrue();
        result.Uid.Should().Be("nonexistentuid");
    }

    // ── PlayerUidsForWorldsAsync ───────────────────────────────────────────────

    [Fact]
    public async Task PlayerUidsForWorldsAsync_NonExistentDirectory_ReturnsEmpty()
    {
        var db = CreateInMemoryDb();
        var sut = BuildSut(db);

        var result = await sut.PlayerUidsForWorldsAsync(["/nonexistent/path"]);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task PlayerUidsForWorldsAsync_EmptyList_ReturnsEmpty()
    {
        var db = CreateInMemoryDb();
        var sut = BuildSut(db);

        var result = await sut.PlayerUidsForWorldsAsync([]);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task PlayerUidsForWorldsAsync_WithValidDatFiles_ReturnsUids()
    {
        var db = CreateInMemoryDb();
        var sut = BuildSut(db);

        // Set up a temp world with playerdata
        string worldDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        string playerDataDir = Path.Combine(worldDir, "playerdata");
        Directory.CreateDirectory(playerDataDir);

        // Valid 32-char hex UUID (no hyphens in filename minus .dat)
        string validUid = "0123456789abcdef0123456789abcdef";
        File.WriteAllText(Path.Combine(playerDataDir, $"{validUid}.dat"), "");

        try
        {
            var result = await sut.PlayerUidsForWorldsAsync([worldDir]);
            result.Should().Contain(validUid);
        }
        finally
        {
            Directory.Delete(worldDir, recursive: true);
        }
    }

    [Fact]
    public async Task PlayerUidsForWorldsAsync_IgnoresInvalidUuidFiles()
    {
        var db = CreateInMemoryDb();
        var sut = BuildSut(db);

        string worldDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        string playerDataDir = Path.Combine(worldDir, "playerdata");
        Directory.CreateDirectory(playerDataDir);

        // Invalid UUID file (not 32 hex chars)
        File.WriteAllText(Path.Combine(playerDataDir, "not-a-uuid.dat"), "");

        try
        {
            var result = await sut.PlayerUidsForWorldsAsync([worldDir]);
            result.Should().BeEmpty();
        }
        finally
        {
            Directory.Delete(worldDir, recursive: true);
        }
    }
}
