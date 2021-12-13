using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAveryCommon.Model.Privileges;

namespace ProjectAvery.Logic.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly ITokenManager _tokenManager;

    public AuthenticationService(ILogger<AuthenticationService> logger, ITokenManager tokenManager)
    {
        _logger = logger;
        _tokenManager = tokenManager;
    }
    
    public IReadOnlySet<IPrivilege> Privileges { get; private set; }

    public void AuthenticateToken(string token)
    {
        _logger.LogDebug($"Token ...{token.Substring(Math.Max(0,token.Length-5))} is trying to authenticate");
        Privileges = _tokenManager.GetPrivilegesForToken(token);
    }

    public bool IsAuthenticated(Type privilegeType) 
    {
        if (Privileges == null)
        {
            throw new UnauthorizedAccessException("Each Request needs to be authorized!");
        }

        // IPrivilege means that any privilege is enough to authenticate
        if (privilegeType == typeof(IPrivilege))
        {
            return Privileges.Any();
        }

        if (!typeof(IPrivilege).IsAssignableFrom(privilegeType))
        {
            throw new ArgumentException("The required privilege needs to be an IPrivilege");
        }

        return Privileges.Any(p => p.GetType().IsAssignableFrom(privilegeType) || p.GetType() == privilegeType);
    }
}