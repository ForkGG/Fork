using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.ServerSettings;

namespace ProjectAvery.Logic.Services.FileServices;

public class FileReaderService : IFileReaderService
{
    private readonly ILogger<FileReaderService> _logger;
    private readonly IFileWriterService _fileWriter;

    public FileReaderService(ILogger<FileReaderService> logger, IFileWriterService fileWriter)
    {
        _logger = logger;
        _fileWriter = fileWriter;
    }

    public async Task<Dictionary<string, string>> ReadVanillaSettingsAsync(string folderPath)
    {
        string propertiesPath = Path.Combine(folderPath, "server.properties");
        FileInfo propertiesFile = new FileInfo(propertiesPath);
        if (!propertiesFile.Exists)
        {
            _logger.LogInformation("Could not find properties file: " + propertiesPath +
                              "\nCreating default server.properties file");
            await _fileWriter.WriteServerSettings(folderPath, new VanillaSettings("world").SettingsDictionary);
        }

        Dictionary<string, string> serverSettings = new Dictionary<string, string>();
        try
        {
            // Open the text file using a stream reader.
            using StreamReader sr = new StreamReader(propertiesFile.FullName);
            string line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                if (!line.StartsWith("#"))
                {
                    string[] args = line.Split('=');
                    serverSettings.Add(args[0], args[1].Replace("\\n", "\n"));
                }
            }

            return serverSettings;
        }
        catch (IOException e)
        {
            _logger.LogError(e, "The file could not be read");
            return null;
        }
    }
}