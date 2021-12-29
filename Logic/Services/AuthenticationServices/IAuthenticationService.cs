using System;
using System.Collections.Generic;
using ForkCommon.Model.Privileges;

namespace Fork.Logic.Services.AuthenticationServices;

/// <summary>
/// A scoped service interface for handling authentication with tokens
/// </summary>
public interface IAuthenticationService
{
    public IReadOnlySet<IPrivilege> Privileges { get; }

    void AuthenticateToken(string token);
    bool IsAuthenticated(Type privilegeType);
}