using System;
using System.Collections.Generic;
using ProjectAveryCommon.Model.Privileges;

namespace ProjectAvery.Logic.Services.Authentication;

/// <summary>
/// A scoped service interface for handling authentication with tokens
/// </summary>
public interface IAuthenticationService
{
    public IReadOnlySet<IPrivilege> Privileges { get; }

    void AuthenticateToken(string token);
    bool IsAuthenticated(Type privilegeType);
}