using System.Collections.Generic;
using ProjectAveryCommon.Model.Privileges;

namespace ProjectAvery.Logic.Managers;

/// <summary>
/// Central unit for managing tokens and privileges for authentication
/// </summary>
public interface ITokenManager
{
    IReadOnlySet<IPrivilege> GetPrivilegesForToken(string token);
}