using System.Collections.Generic;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Privileges;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fork.Controllers;

public class CreateEntityController : AbstractRestController
{
    private readonly ServerVersionManager _serverVersionManager;

    public CreateEntityController(ILogger<CreateEntityController> logger, ServerVersionManager serverVersionManager) :
        base(logger)
    {
        _serverVersionManager = serverVersionManager;
    }

    [HttpGet("types")]
    [Privileges(typeof(IPrivilege))]
    public List<VersionType> GetAvailableVersionTypes()
    {
        return _serverVersionManager.SupportedVersionTypes;
    }

    [HttpGet("{versionType}/versions")]
    [Privileges(typeof(IPrivilege))]
    public async Task<List<ServerVersion>> GetVersionsForType([FromRoute] VersionType versionType)
    {
        return await _serverVersionManager.GetServerVersionsForType(versionType);
    }
}