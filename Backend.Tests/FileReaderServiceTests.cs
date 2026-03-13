using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Fork.Logic.Services.FileServices;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fork.Backend.Tests;

public class FileReaderServiceTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private readonly FileWriterService _writer = new();
    private readonly FileReaderService _sut;

    public FileReaderServiceTests()
    {
        Directory.CreateDirectory(_tempDir);
        _sut = new FileReaderService(NullLogger<FileReaderService>.Instance, _writer);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    // ── ReadVanillaSettingsAsync ───────────────────────────────────────────────

    [Fact]
    public async Task ReadVanillaSettings_ParsesKeyValuePairs()
    {
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "server.properties"),
            "max-players=50\r\nserver-port=25565\r\n");

        var result = await _sut.ReadVanillaSettingsAsync(_tempDir);

        result["max-players"].Should().Be("50");
        result["server-port"].Should().Be("25565");
    }

    [Fact]
    public async Task ReadVanillaSettings_SkipsCommentLines()
    {
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "server.properties"),
            "#This is a comment\npvp=true\n");

        var result = await _sut.ReadVanillaSettingsAsync(_tempDir);

        result.Should().NotContainKey("#This is a comment");
        result["pvp"].Should().Be("true");
    }

    [Fact]
    public async Task ReadVanillaSettings_UnescapesNewlines()
    {
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "server.properties"),
            @"motd=Line1\nLine2" + "\n");

        var result = await _sut.ReadVanillaSettingsAsync(_tempDir);

        result["motd"].Should().Be("Line1\nLine2");
    }

    [Fact]
    public async Task ReadVanillaSettings_MissingFile_CreatesDefaultAndReturnsSettings()
    {
        // No server.properties exists — should auto-create defaults
        var result = await _sut.ReadVanillaSettingsAsync(_tempDir);

        result.Should().NotBeEmpty();
        result.Should().ContainKey("server-port");
    }

    // ── ReadBanListTxt ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ReadBanListTxt_ReturnsEmptyWhenFileMissing()
    {
        var result = await _sut.ReadBanListTxt(_tempDir);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadBanListTxt_ParsesPipeDelimitedUids()
    {
        string content =
            "#Ban list\n" +
            "abc123|reason|source|created|expires\n" +
            "def456|reason|source|created|expires\n";
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "banned-players.txt"), content);

        var result = await _sut.ReadBanListTxt(_tempDir);

        result.Should().BeEquivalentTo(["abc123", "def456"]);
    }

    [Fact]
    public async Task ReadBanListTxt_SkipsLinesWithWrongFieldCount()
    {
        string content = "only|three|fields\n" +
                         "valid|reason|source|created|expires\n";
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "banned-players.txt"), content);

        var result = await _sut.ReadBanListTxt(_tempDir);

        result.Should().ContainSingle().Which.Should().Be("valid");
    }

    [Fact]
    public async Task ReadBanListTxt_SkipsEmptyLines()
    {
        string content = "uid1|a|b|c|d\n\nuid2|a|b|c|d\n";
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "banned-players.txt"), content);

        var result = await _sut.ReadBanListTxt(_tempDir);

        result.Should().HaveCount(2);
    }

    // ── ReadBanListJson ────────────────────────────────────────────────────────

    [Fact]
    public async Task ReadBanListJson_ReturnsEmptyWhenFileMissing()
    {
        var result = await _sut.ReadBanListJson(_tempDir);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadBanListJson_ParsesUuids()
    {
        string json = """
            [
              { "uuid": "abc-123", "name": "Player1", "created": "2024-01-01", "source": "Server", "expires": "forever", "reason": "" },
              { "uuid": "def-456", "name": "Player2", "created": "2024-01-01", "source": "Server", "expires": "forever", "reason": "" }
            ]
            """;
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "banned-players.json"), json);

        var result = await _sut.ReadBanListJson(_tempDir);

        result.Should().BeEquivalentTo(["abc-123", "def-456"]);
    }

    [Fact]
    public async Task ReadBanListJson_EmptyArray_ReturnsEmpty()
    {
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "banned-players.json"), "[]");
        var result = await _sut.ReadBanListJson(_tempDir);
        result.Should().BeEmpty();
    }

    // ── ReadWhiteListJson ──────────────────────────────────────────────────────

    [Fact]
    public async Task ReadWhiteListJson_ReturnsEmptyWhenFileMissing()
    {
        var result = await _sut.ReadWhiteListJson(_tempDir);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadWhiteListJson_ParsesUuids()
    {
        string json = """
            [
              { "uuid": "player-uuid-1", "name": "Steve" },
              { "uuid": "player-uuid-2", "name": "Alex" }
            ]
            """;
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "whitelist.json"), json);

        var result = await _sut.ReadWhiteListJson(_tempDir);

        result.Should().BeEquivalentTo(["player-uuid-1", "player-uuid-2"]);
    }

    // ── ReadWhiteListTxt ───────────────────────────────────────────────────────

    [Fact]
    public async Task ReadWhiteListTxt_ReturnsEmptyWhenFileMissing()
    {
        var result = await _sut.ReadWhiteListTxt(_tempDir);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadWhiteListTxt_ParsesPlayerNames()
    {
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "white-list.txt"),
            "#Comment\nSteve\nAlex\n");

        var result = await _sut.ReadWhiteListTxt(_tempDir);

        result.Should().BeEquivalentTo(["Steve", "Alex"]);
    }

    [Fact]
    public async Task ReadWhiteListTxt_StripsTrailingCommas()
    {
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "white-list.txt"), "Steve,\nAlex,\n");

        var result = await _sut.ReadWhiteListTxt(_tempDir);

        result.Should().BeEquivalentTo(["Steve", "Alex"]);
    }

    // ── ReadOpListJson ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ReadOpListJson_ReturnsEmptyWhenFileMissing()
    {
        var result = await _sut.ReadOpListJson(_tempDir);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadOpListJson_ParsesUuids()
    {
        string json = """[{"uuid":"op-uuid-1","name":"Admin","level":4,"bypassesPlayerLimit":false}]""";
        await File.WriteAllTextAsync(Path.Combine(_tempDir, "ops.json"), json);

        var result = await _sut.ReadOpListJson(_tempDir);

        result.Should().ContainSingle().Which.Should().Be("op-uuid-1");
    }
}
