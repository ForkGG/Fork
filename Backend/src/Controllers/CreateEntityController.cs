using System.Collections.Generic;
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
    public List<ServerVersion> GetVersionsForType([FromRoute] VersionType versionType)
    {
        return new List<ServerVersion>
        {
            new()
            {
                Id = 0,
                Type = versionType,
                Version = "1.10.2",
                JarLink =
                    "https://piston-data.mojang.com/v1/objects/15c777e2cfe0556eef19aab534b186c0c6f277e1/server.jar"
            },
            new()
            {
                Id = 0,
                Type = versionType,
                Version = "1.10.1",
                JarLink =
                    "https://piston-data.mojang.com/v1/objects/15c777e2cfe0556eef19aab534b186c0c6f277e1/server.jar"
            }
        };
    }
}