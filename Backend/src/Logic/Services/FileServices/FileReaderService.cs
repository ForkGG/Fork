using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ForkCommon.Model.Application.Exceptions;
using ForkCommon.Model.Entity.Pocos.ServerSettings;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Fork.Logic.Services.FileServices;

public class FileReaderService
{
    private readonly FileWriterService _fileWriter;
    private readonly ILogger<FileReaderService> _logger;

    public FileReaderService(ILogger<FileReaderService> logger, FileWriterService fileWriter)
    {
        _logger = logger;
        _fileWriter = fileWriter;
    }

    public async Task<Dictionary<string, string>> ReadVanillaSettingsAsync(string folderPath)
    {
        string propertiesPath = Path.Combine(folderPath, "server.properties");
        FileInfo propertiesFile = new(propertiesPath);
        if (!propertiesFile.Exists)
        {
            _logger.LogInformation("Could not find properties file: " + propertiesPath +
                                   "\nCreating default server.properties file");
            await _fileWriter.WriteServerSettings(folderPath, new VanillaSettings("world").SettingsDictionary);
        }

        Dictionary<string, string> serverSettings = new();
        try
        {
            // Open the text file using a stream reader.
            using StreamReader sr = new(propertiesFile.FullName);
            while (await sr.ReadLineAsync() is { } line)
                if (!line.StartsWith("#"))
                {
                    string[] args = line.Split('=');
                    serverSettings.Add(args[0], args[1].Replace("\\n", "\n"));
                }

            return serverSettings;
        }
        catch (IOException e)
        {
            _logger.LogError(e, "The file could not be read");
            throw new ForkException(e);
        }
    }

    public async Task<List<string>> ReadWhiteListTxt(string serverPath)
    {
        string path = Path.Combine(serverPath, "white-list.txt");
        return await ReadRoleTxtFile(path);
    }

    public async Task<List<string>> ReadWhiteListJson(string serverPath)
    {
        string path = Path.Combine(serverPath, "whitelist.json");
        return await ReadRoleJsonFile(path);
    }

    public async Task<List<string>> ReadOpListTxt(string serverPath)
    {
        string path = Path.Combine(serverPath, "ops.txt");
        return await ReadRoleTxtFile(path);
    }

    public async Task<List<string>> ReadOpListJson(string serverPath)
    {
        string path = Path.Combine(serverPath, "ops.json");
        return await ReadRoleJsonFile(path);
    }

    public async Task<List<string>> ReadBanListTxt(string serverPath)
    {
        string path = Path.Combine(serverPath, "banned-players.txt");

        // We can't use generic method here, because of the structure of this file...
        if (!File.Exists(path))
        {
            return new List<string>();
        }

        List<string> list = new();
        FileStream fs = new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader sr = new(fs);
        while (await sr.ReadLineAsync() is { } line)
        {
            if (line.StartsWith("#") || line.Length == 0)
            {
                continue;
            }

            string[] splitLine = line.Split('|');
            if (splitLine.Length != 5)
            {
                continue;
            }

            list.Add(splitLine[0]);
        }

        return list;
    }

    public async Task<List<string>> ReadBanListJson(string serverPath)
    {
        string path = Path.Combine(serverPath, "banned-players.json");
        return await ReadRoleJsonFile(path);
    }

    private async Task<List<string>> ReadRoleTxtFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<string>();
        }

        List<string> result = new();
        FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader sr = new(fs);
        while (await sr.ReadLineAsync() is { } line)
        {
            if (line.StartsWith("#") || line.Length == 0)
            {
                continue;
            }

            if (line.EndsWith(","))
            {
                line = line.Remove(line.Length - 1);
            }

            result.Add(line);
        }

        return result;
    }

    private async Task<List<string>> ReadRoleJsonFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        string json;
        await using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (StreamReader sr = new(fs))
            json = await sr.ReadToEndAsync();

        dynamic? deserialized = JsonConvert.DeserializeObject(json);
        List<string> uuids = [];
        if (deserialized is JArray deserializedArray)
        {
            dynamic[]? players = deserializedArray.ToObject<dynamic[]>();
            IEnumerable<string?>? matchingPlayers = players?.Select(player => player.uuid.Value as string);
            if (matchingPlayers != null)
            {
                uuids.AddRange(matchingPlayers.OfType<string>());
            }
        }


        return uuids;
    }
}