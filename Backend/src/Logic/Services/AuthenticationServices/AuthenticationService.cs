using System;
using System.Collections.Generic;
using System.Linq;
using Fork.Logic.Managers;
using ForkCommon.Model.Privileges;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Services.AuthenticationServices;

public class AuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly TokenManager _tokenManager;

    public AuthenticationService(ILogger<AuthenticationService> logger, TokenManager tokenManager)
    {
        _logger = logger;
        _tokenManager = tokenManager;
    }

    public IReadOnlySet<IPrivilege>? Privileges { get; private set; }

    public void AuthenticateToken(string token)
    {
        _logger.LogDebug($"Token ...{token.Substring(Math.Max(0, token.Length - 5))} is trying to authenticate");
        Privileges = _tokenManager.GetPrivilegesForToken(token);
    }

    // TODO CKE ask for entityId if entity right is asked for (maybe rework whole privilege system)
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

        return Privileges.Any(p =>
            p is AdminPrivilege || p.GetType().IsAssignableFrom(privilegeType) || p.GetType() == privilegeType);
    }
}