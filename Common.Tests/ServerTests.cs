using FluentAssertions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Automation;
using ForkCommon.Model.Entity.Pocos.Settings;

namespace ForkCommon.Tests;

public class ServerTests
{
    private static Server MakeServer(string name = "TestServer", string version = "1.20.4")
    {
        var sv = new ServerVersion { Type = VersionType.Vanilla, Version = version };
        return new Server(name, sv, new VanillaSettings("world"), new JavaSettings());
    }

    [Fact]
    public void Constructor_SetsEntityName()
    {
        var server = MakeServer("MyServer");
        server.EntitySettings.EntityName.Should().Be("MyServer");
    }

    [Fact]
    public void Constructor_SetsServerVersion()
    {
        var server = MakeServer(version: "1.21.0");
        server.EntitySettings.ServerVersion!.Version.Should().Be("1.21.0");
    }

    [Fact]
    public void Constructor_InitializedIsFalse()
    {
        MakeServer().Initialized.Should().BeFalse();
    }

    [Fact]
    public void Constructor_StatusIsStopped()
    {
        MakeServer().Status.Should().Be(EntityStatus.Stopped);
    }

    [Fact]
    public void Constructor_Creates8AutomationTimes()
    {
        MakeServer().EntitySettings.AutomationTimes.Should().HaveCount(8);
    }

    [Fact]
    public void Constructor_AllAutomationTimesDisabled()
    {
        MakeServer().EntitySettings.AutomationTimes.Should().AllSatisfy(t => t.Enabled.Should().BeFalse());
    }

    [Fact]
    public void Constructor_AutomationTimesContainAllTypes()
    {
        var times = MakeServer().EntitySettings.AutomationTimes;

        times.Count(t => t.Type == AutomationType.Restart).Should().Be(4);
        times.Count(t => t.Type == AutomationType.Stop).Should().Be(2);
        times.Count(t => t.Type == AutomationType.Start).Should().Be(2);
    }

    [Fact]
    public void Constructor_ServerPlayersIsEmpty()
    {
        MakeServer().ServerPlayers.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ConsoleMessagesIsEmpty()
    {
        MakeServer().ConsoleMessages.Should().BeEmpty();
    }

    [Fact]
    public void FullName_ReturnsCombinedNameAndVersion()
    {
        var server = MakeServer("Survival", "1.20.4");
        server.FullName.Should().Be("Survival (1.20.4)");
    }

    [Fact]
    public void ToString_TruncatesLongNames()
    {
        var server = MakeServer("AVeryLongServerName", "1.20.4");
        server.ToString().Should().Be("AVeryLongS (1.20.4)");
    }

    [Fact]
    public void ToString_ShortNameNotTruncated()
    {
        var server = MakeServer("Short", "1.20.4");
        server.ToString().Should().Be("Short (1.20.4)");
    }

    [Fact]
    public void JarLink_ReflectsServerVersionJarLink()
    {
        var server = MakeServer();
        server.EntitySettings.ServerVersion!.JarLink = "http://example.com/server.jar";
        server.JarLink.Should().Be("http://example.com/server.jar");
    }

    [Fact]
    public void AutoSetSha1_DefaultIsTrue()
    {
        MakeServer().AutoSetSha1.Should().BeTrue();
    }
}
