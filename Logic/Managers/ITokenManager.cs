using System.Collections.Generic;
using ProjectAvery.Logic.Model.Enums;

namespace ProjectAvery.Logic.Managers;

/// <summary>
/// Central unit for managing tokens and privileges for authentication
/// </summary>
public interface ITokenManager
{
    IReadOnlySet<Privilege> GetPrivilegesForToken(string token);
}