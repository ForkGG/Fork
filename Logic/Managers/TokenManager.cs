using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ProjectAvery.Logic.Model.Enums;
using ProjectAveryCommon.ExtensionMethods;

namespace ProjectAvery.Logic.Managers;

public class TokenManager : ITokenManager
{
    private readonly ILogger<TokenManager> _logger;

    public TokenManager(ILogger<TokenManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get a Set of all privileges of a given token
    /// </summary>
    /// <returns>Set of privileges or <c>null</c> if the token is not valid</returns>
    public IReadOnlySet<Privilege> GetPrivilegesForToken(string token)
    {
        // TODO CKE
        return new HashSet<Privilege>(EnumExtensions.GetValues<Privilege>());
    }
}