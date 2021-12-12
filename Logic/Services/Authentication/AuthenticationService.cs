using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Managers;
using ProjectAvery.Logic.Model.Enums;

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
    
    private IReadOnlySet<Privilege> Privileges { get; set; }

    public void AuthenticateToken(string token)
    {
        _logger.LogDebug($"Token ...{token.Substring(Math.Max(0,token.Length-5))} is trying to authenticate");
        Privileges = _tokenManager.GetPrivilegesForToken(token);
    }

    public bool IsAuthenticated(Privilege privilege)
    {
        if (Privileges == null)
        {
            throw new UnauthorizedAccessException("Each Request needs to be authorized!");
        }

        return Privileges.Contains(privilege);
    }
}