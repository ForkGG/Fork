using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAvery.Logic.Services.FileServices;
using ProjectAveryCommon.Model.Entity.Enums;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.ServerSettings;

namespace ProjectAvery.Logic.Services.EntityServices;

public class EntityPostProcessingService : IEntityPostProcessingService
{
    private readonly ILogger<EntityPostProcessingService> _logger;
    private readonly IFileReaderService _fileReader;
    private readonly IApplicationManager _application;

    public EntityPostProcessingService(ILogger<EntityPostProcessingService> logger, IFileReaderService fileReader,
        IApplicationManager application)
    {
        _logger = logger;
        _fileReader = fileReader;
        _application = application;
    }

    public async Task PostProcessEntity(IEntity entity)
    {
        await DoEntityPostProcessing(entity);
        
        if (entity is Server server)
        {
            await DoServerPostProcessing(server);
        }
    }

    private async Task DoEntityPostProcessing(IEntity entity)
    {
        await Task.CompletedTask;
    }

    private async Task DoServerPostProcessing(Server server)
    {
        server.VanillaSettings =
            new VanillaSettings(
                await _fileReader.ReadVanillaSettingsAsync(Path.Combine(_application.EntityPath, server.Name)));
    }
}