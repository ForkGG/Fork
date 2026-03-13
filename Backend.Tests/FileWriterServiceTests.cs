using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Fork.Logic.Services.FileServices;

namespace Fork.Backend.Tests;

public class FileWriterServiceTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private readonly FileWriterService _sut = new();

    public FileWriterServiceTests()
    {
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    // ── WriteEula ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task WriteEula_CreatesEulaTxt()
    {
        await _sut.WriteEula(_tempDir);

        File.Exists(Path.Combine(_tempDir, "eula.txt")).Should().BeTrue();
    }

    [Fact]
    public async Task WriteEula_ContainsEulaTrue()
    {
        await _sut.WriteEula(_tempDir);

        string content = await File.ReadAllTextAsync(Path.Combine(_tempDir, "eula.txt"));
        content.Should().Contain("eula=true");
    }

    [Fact]
    public async Task WriteEula_ContainsCommentLine()
    {
        await _sut.WriteEula(_tempDir);

        string content = await File.ReadAllTextAsync(Path.Combine(_tempDir, "eula.txt"));
        content.Should().Contain("#By changing the setting below to TRUE");
    }

    [Fact]
    public async Task WriteEula_CreatesDirectoryIfMissing()
    {
        string subDir = Path.Combine(_tempDir, "new_server");
        subDir.Should().NotBeNull();

        await _sut.WriteEula(subDir);

        Directory.Exists(subDir).Should().BeTrue();
        File.Exists(Path.Combine(subDir, "eula.txt")).Should().BeTrue();
    }

    // ── WriteServerSettings ────────────────────────────────────────────────────

    [Fact]
    public async Task WriteServerSettings_CreatesServerPropertiesFile()
    {
        var settings = new Dictionary<string, string> { ["server-port"] = "25565" };

        await _sut.WriteServerSettings(_tempDir, settings);

        File.Exists(Path.Combine(_tempDir, "server.properties")).Should().BeTrue();
    }

    [Fact]
    public async Task WriteServerSettings_WritesKeyValuePairs()
    {
        var settings = new Dictionary<string, string>
        {
            ["max-players"] = "20",
            ["pvp"] = "true",
            ["level-name"] = "world"
        };

        await _sut.WriteServerSettings(_tempDir, settings);

        string content = await File.ReadAllTextAsync(Path.Combine(_tempDir, "server.properties"), Encoding.UTF8);
        content.Should().Contain("max-players=20");
        content.Should().Contain("pvp=true");
        content.Should().Contain("level-name=world");
    }

    [Fact]
    public async Task WriteServerSettings_EscapesNewlines()
    {
        var settings = new Dictionary<string, string>
        {
            ["motd"] = "Line1\nLine2"
        };

        await _sut.WriteServerSettings(_tempDir, settings);

        string content = await File.ReadAllTextAsync(Path.Combine(_tempDir, "server.properties"), Encoding.UTF8);
        content.Should().Contain(@"motd=Line1\nLine2");
    }

    [Fact]
    public async Task WriteServerSettings_StripsCarriageReturns()
    {
        var settings = new Dictionary<string, string>
        {
            ["motd"] = "Hello\r\nWorld"
        };

        await _sut.WriteServerSettings(_tempDir, settings);

        string content = await File.ReadAllTextAsync(Path.Combine(_tempDir, "server.properties"), Encoding.UTF8);
        // \r should be stripped, \n should become \\n
        content.Should().Contain(@"motd=Hello\nWorld");
    }

    [Fact]
    public async Task WriteServerSettings_StartsWithComment()
    {
        await _sut.WriteServerSettings(_tempDir, new Dictionary<string, string>());

        string[] lines = await File.ReadAllLinesAsync(Path.Combine(_tempDir, "server.properties"));
        lines[0].Should().Be("#Minecraft server properties");
    }

    // ── IsFileWritable ─────────────────────────────────────────────────────────

    [Fact]
    public void IsFileWritable_UnlockedFile_ReturnsTrue()
    {
        string path = Path.Combine(_tempDir, "writable.txt");
        File.WriteAllText(path, "");

        FileWriterService.IsFileWritable(new FileInfo(path)).Should().BeTrue();
    }

    [Fact]
    public void IsFileWritable_LockedFile_ReturnsFalse()
    {
        string path = Path.Combine(_tempDir, "locked.txt");
        using FileStream fs = File.Open(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);

        FileWriterService.IsFileWritable(new FileInfo(path)).Should().BeFalse();
    }
}
