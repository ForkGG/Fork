using FluentAssertions;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;

namespace ForkCommon.Tests;

public class ServerVersionTests
{
    private static ServerVersion Make(string version, VersionType type = VersionType.Vanilla) =>
        new() { Version = version, Type = type };

    // ── CompareTo ──────────────────────────────────────────────────────────────

    [Fact]
    public void CompareTo_SameVersion_ReturnsZero()
    {
        Make("1.20.4").CompareTo(Make("1.20.4")).Should().Be(0);
    }

    [Fact]
    public void CompareTo_HigherMajor_ReturnsPositive()
    {
        Make("2.0.0").CompareTo(Make("1.21.0")).Should().BePositive();
    }

    [Fact]
    public void CompareTo_LowerMajor_ReturnsNegative()
    {
        Make("1.0.0").CompareTo(Make("2.0.0")).Should().BeNegative();
    }

    [Fact]
    public void CompareTo_HigherMinor_ReturnsPositive()
    {
        Make("1.21.0").CompareTo(Make("1.20.0")).Should().BePositive();
    }

    [Fact]
    public void CompareTo_LowerMinor_ReturnsNegative()
    {
        Make("1.16.5").CompareTo(Make("1.17.0")).Should().BeNegative();
    }

    [Fact]
    public void CompareTo_HigherPatch_ReturnsPositive()
    {
        Make("1.20.5").CompareTo(Make("1.20.4")).Should().BePositive();
    }

    [Fact]
    public void CompareTo_LowerPatch_ReturnsNegative()
    {
        Make("1.20.3").CompareTo(Make("1.20.4")).Should().BeNegative();
    }

    [Fact]
    public void CompareTo_TwoPartVsThreePart_WorksCorrectly()
    {
        // "1.17" has an implicit third segment of 0 (due to padding with "0")
        Make("1.17").CompareTo(Make("1.16.5")).Should().BePositive();
        Make("1.16").CompareTo(Make("1.17.0")).Should().BeNegative();
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        Make("1.20.0").CompareTo(null).Should().BePositive();
    }

    [Fact]
    public void CompareTo_NonServerVersion_Throws()
    {
        Action act = () => Make("1.20.0").CompareTo("not a version");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CompareTo_Version1_13Constants_WorkCorrectly()
    {
        Make("1.13").CompareTo(ServerVersion.Version1_13).Should().Be(0);
        Make("1.14").CompareTo(ServerVersion.Version1_13).Should().BePositive();
        Make("1.12").CompareTo(ServerVersion.Version1_13).Should().BeNegative();
    }

    [Fact]
    public void CompareTo_Version1_18Constants_WorkCorrectly()
    {
        Make("1.18").CompareTo(ServerVersion.Version1_18).Should().Be(0);
        Make("1.19").CompareTo(ServerVersion.Version1_18).Should().BePositive();
    }

    // ── IsEqualOrGreaterThan ───────────────────────────────────────────────────

    [Fact]
    public void IsEqualOrGreaterThan_StrictlyGreater_ReturnsTrue()
    {
        Make("1.18").IsEqualOrGreaterThan(Make("1.17")).Should().BeTrue();
    }

    [Fact]
    public void IsEqualOrGreaterThan_Equal_ReturnsFalse_KnownBug()
    {
        // BUG: IsEqualOrGreaterThan returns CompareTo(other) > 0, which is false for equal versions.
        // This test documents the existing (incorrect) behavior.
        Make("1.17").IsEqualOrGreaterThan(Make("1.17")).Should().BeFalse("known bug: equal versions return false");
    }

    [Fact]
    public void IsEqualOrGreaterThan_Strictly_Lower_ReturnsFalse()
    {
        Make("1.16").IsEqualOrGreaterThan(Make("1.17")).Should().BeFalse();
    }

    // ── Equality ───────────────────────────────────────────────────────────────

    [Fact]
    public void Equals_SameTypeVersionJarLink_ReturnsTrue()
    {
        var a = new ServerVersion { Type = VersionType.Vanilla, Version = "1.20.4", JarLink = "http://example.com" };
        var b = new ServerVersion { Type = VersionType.Vanilla, Version = "1.20.4", JarLink = "http://example.com" };
        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        var a = new ServerVersion { Type = VersionType.Vanilla, Version = "1.20.4" };
        var b = new ServerVersion { Type = VersionType.Paper, Version = "1.20.4" };
        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferentVersion_ReturnsFalse()
    {
        var a = new ServerVersion { Type = VersionType.Vanilla, Version = "1.20.4" };
        var b = new ServerVersion { Type = VersionType.Vanilla, Version = "1.20.3" };
        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        Make("1.20.4").Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var v = Make("1.20.4");
        v.Equals(v).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_EqualObjects_SameHash()
    {
        var a = new ServerVersion { Type = VersionType.Vanilla, Version = "1.20.4", JarLink = "http://x" };
        var b = new ServerVersion { Type = VersionType.Vanilla, Version = "1.20.4", JarLink = "http://x" };
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    // ── Clone ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Clone_ProducesEqualButNotSameInstance()
    {
        var original = new ServerVersion { Type = VersionType.Paper, Version = "1.20.4", JarLink = "http://x", Build = 42 };
        var clone = original.Clone();

        clone.Should().NotBeSameAs(original);
        clone.Type.Should().Be(original.Type);
        clone.Version.Should().Be(original.Version);
        clone.JarLink.Should().Be(original.JarLink);
        clone.Build.Should().Be(original.Build);
    }

    // ── Computed Properties ────────────────────────────────────────────────────

    [Theory]
    [InlineData(VersionType.Waterfall, true)]
    [InlineData(VersionType.Vanilla, false)]
    [InlineData(VersionType.Paper, false)]
    public void IsProxy_OnlyTrueForWaterfall(VersionType type, bool expected)
    {
        new ServerVersion { Type = type }.IsProxy.Should().Be(expected);
    }

    [Theory]
    [InlineData(VersionType.Paper, true)]
    [InlineData(VersionType.Vanilla, false)]
    [InlineData(VersionType.Waterfall, false)]
    public void SupportBuilds_OnlyTrueForPaper(VersionType type, bool expected)
    {
        new ServerVersion { Type = type }.SupportBuilds.Should().Be(expected);
    }

    [Theory]
    [InlineData(VersionType.Paper, true)]
    [InlineData(VersionType.Spigot, true)]
    [InlineData(VersionType.Vanilla, false)]
    [InlineData(VersionType.Waterfall, false)]
    public void HasPlugins_TrueForPaperAndSpigot(VersionType type, bool expected)
    {
        new ServerVersion { Type = type }.HasPlugins.Should().Be(expected);
    }

    [Fact]
    public void ToString_ReturnsVersion()
    {
        Make("1.20.4").ToString().Should().Be("1.20.4");
    }

    [Fact]
    public void ToString_NullVersion_ReturnsEmptyString()
    {
        new ServerVersion { Version = null }.ToString().Should().Be("");
    }
}
