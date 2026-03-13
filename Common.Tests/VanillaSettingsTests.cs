using FluentAssertions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos.Settings;

namespace ForkCommon.Tests;

public class VanillaSettingsTests
{
    // ── Default values ─────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_WithLevelName_SetsExpectedDefaults()
    {
        var s = new VanillaSettings("myworld");

        s.LevelName.Should().Be("myworld");
        s.SpawnProtection.Should().Be(16);
        s.OpPermissionLevel.Should().Be(4);
        s.MaxPlayers.Should().Be(20);
        s.ViewDistance.Should().Be(10);
        s.ServerPort.Should().Be(25565);
        s.RconPort.Should().Be(25575);
        s.QueryPort.Should().Be(25565);
        s.MaxBuildHeight.Should().Be(256);
        s.NetworkCompressionThreshold.Should().Be(256);
        s.RateLimit.Should().Be(0);
        s.MaxTickTime.Should().Be(60000L);
        s.PlayerIdleTimeout.Should().Be(0L);
        s.MaxWorldSize.Should().Be(29999984);
    }

    [Fact]
    public void Constructor_WithLevelName_SetsExpectedBoolDefaults()
    {
        var s = new VanillaSettings("world");

        s.ForceGamemode.Should().BeFalse();
        s.AllowNether.Should().BeTrue();
        s.EnforceWhitelist.Should().BeFalse();
        s.BcastConsoleToOps.Should().BeTrue();
        s.EnableQuery.Should().BeTrue();
        s.SpawnMonsters.Should().BeTrue();
        s.BcastRconToOps.Should().BeTrue();
        s.Pvp.Should().BeTrue();
        s.SnooperEnabled.Should().BeTrue();
        s.Hardcore.Should().BeFalse();
        s.EnableCommandBlock.Should().BeFalse();
        s.SpawnNpcs.Should().BeTrue();
        s.AllowFlight.Should().BeFalse();
        s.SpawnAnimals.Should().BeTrue();
        s.Whitelist.Should().BeFalse();
        s.GenerateStructures.Should().BeTrue();
        s.OnlineMode.Should().BeTrue();
        s.UseNativeTransport.Should().BeTrue();
        s.PreventProxyConnections.Should().BeFalse();
        s.EnableRcon.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithLevelName_SetsEnumDefaults()
    {
        var s = new VanillaSettings("world");

        s.CurrGamemode.Should().Be(Gamemode.Survival);
        s.CurrDifficulty.Should().Be(Difficulty.Easy);
        s.CurrLevelType.Should().Be(LevelType.Default);
    }

    // ── Bool property round-trips ──────────────────────────────────────────────

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ForceGamemode_RoundTrip(bool value)
    {
        var s = new VanillaSettings("world");
        s.ForceGamemode = value;
        s.ForceGamemode.Should().Be(value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Whitelist_RoundTrip(bool value)
    {
        var s = new VanillaSettings("world");
        s.Whitelist = value;
        s.Whitelist.Should().Be(value);
    }

    [Fact]
    public void BoolProperties_StoredAsLowercase()
    {
        var s = new VanillaSettings("world");
        s.Pvp = true;
        s.SettingsDictionary["pvp"].Should().Be("true");

        s.Pvp = false;
        s.SettingsDictionary["pvp"].Should().Be("false");
    }

    // ── Integer property round-trips ───────────────────────────────────────────

    [Fact]
    public void MaxPlayers_RoundTrip()
    {
        var s = new VanillaSettings("world");
        s.MaxPlayers = 100;
        s.MaxPlayers.Should().Be(100);
    }

    [Fact]
    public void ServerPort_RoundTrip()
    {
        var s = new VanillaSettings("world");
        s.ServerPort = 19132;
        s.ServerPort.Should().Be(19132);
    }

    [Fact]
    public void ViewDistance_RoundTrip()
    {
        var s = new VanillaSettings("world");
        s.ViewDistance = 32;
        s.ViewDistance.Should().Be(32);
    }

    // ── Long property round-trips ──────────────────────────────────────────────

    [Fact]
    public void MaxTickTime_RoundTrip()
    {
        var s = new VanillaSettings("world");
        s.MaxTickTime = 120000L;
        s.MaxTickTime.Should().Be(120000L);
    }

    [Fact]
    public void MaxWorldSize_RoundTrip()
    {
        var s = new VanillaSettings("world");
        s.MaxWorldSize = 10000000;
        s.MaxWorldSize.Should().Be(10000000);
    }

    // ── String property round-trips ────────────────────────────────────────────

    [Fact]
    public void Motd_RoundTrip()
    {
        var s = new VanillaSettings("world");
        const string motd = "My Awesome Server";
        s.Motd = motd;
        s.Motd.Should().Be(motd);
    }

    [Fact]
    public void LevelSeed_RoundTrip()
    {
        var s = new VanillaSettings("world");
        s.LevelSeed = "12345678";
        s.LevelSeed.Should().Be("12345678");
    }

    [Fact]
    public void ResourcePack_RoundTrip()
    {
        var s = new VanillaSettings("world");
        s.ResourcePack = "https://example.com/pack.zip";
        s.ResourcePack.Should().Be("https://example.com/pack.zip");
    }

    // ── Enum property round-trips ──────────────────────────────────────────────

    [Theory]
    [InlineData(Gamemode.Survival)]
    [InlineData(Gamemode.Creative)]
    [InlineData(Gamemode.Adventure)]
    [InlineData(Gamemode.Spectator)]
    public void CurrGamemode_RoundTrip(Gamemode mode)
    {
        var s = new VanillaSettings("world");
        s.CurrGamemode = mode;
        s.CurrGamemode.Should().Be(mode);
    }

    [Fact]
    public void CurrGamemode_StoredAsLowercase()
    {
        var s = new VanillaSettings("world");
        s.CurrGamemode = Gamemode.Creative;
        s.SettingsDictionary["gamemode"].Should().Be("creative");
    }

    [Theory]
    [InlineData(Difficulty.Peaceful)]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Normal)]
    [InlineData(Difficulty.Hard)]
    public void CurrDifficulty_RoundTrip(Difficulty difficulty)
    {
        var s = new VanillaSettings("world");
        s.CurrDifficulty = difficulty;
        s.CurrDifficulty.Should().Be(difficulty);
    }

    [Fact]
    public void CurrDifficulty_StoredAsLowercase()
    {
        var s = new VanillaSettings("world");
        s.CurrDifficulty = Difficulty.Normal;
        s.SettingsDictionary["difficulty"].Should().Be("normal");
    }

    [Theory]
    [InlineData(LevelType.Default)]
    [InlineData(LevelType.Flat)]
    [InlineData(LevelType.Amplified)]
    public void CurrLevelType_RoundTrip(LevelType levelType)
    {
        var s = new VanillaSettings("world");
        s.CurrLevelType = levelType;
        s.CurrLevelType.Should().Be(levelType);
    }

    // ── Dictionary-based constructor ───────────────────────────────────────────

    [Fact]
    public void DictionaryConstructor_OverridesDefaults()
    {
        var dict = new Dictionary<string, string>
        {
            ["max-players"] = "50",
            ["pvp"] = "false",
            ["gamemode"] = "creative",
            ["LevelName"] = "survival_world"
        };

        var s = new VanillaSettings(dict);

        s.MaxPlayers.Should().Be(50);
        s.Pvp.Should().BeFalse();
        s.CurrGamemode.Should().Be(Gamemode.Creative);
        s.LevelName.Should().Be("survival_world");
    }

    [Fact]
    public void DictionaryConstructor_MissingLevelName_DefaultsToWorld()
    {
        var s = new VanillaSettings(new Dictionary<string, string>());
        s.LevelName.Should().Be("world");
    }
}
