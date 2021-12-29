using System.Collections.Generic;
using ForkCommon.Model.Privileges;

namespace Fork.Logic.Managers;

/// <summary>
/// Central unit for managing tokens and privileges for authentication
/// </summary>
public interface ITokenManager
{
    IReadOnlySet<IPrivilege> GetPrivilegesForToken(string token);
}