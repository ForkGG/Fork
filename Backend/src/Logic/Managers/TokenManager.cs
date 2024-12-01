using System.Collections.Generic;
using ForkCommon.Model.Privileges;
using Microsoft.Extensions.Logging;

namespace Fork.Logic.Managers;

public class TokenManager
{
    private readonly ILogger<TokenManager> _logger;

    public TokenManager(ILogger<TokenManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     Get a Set of all privileges of a given token
    /// </summary>
    /// <returns>Set of privileges or <c>null</c> if the token is not valid</returns>
    public IReadOnlySet<IPrivilege> GetPrivilegesForToken(string token)
    {
        // TODO CKE
        return new HashSet<IPrivilege> { new AdminPrivilege() };
    }
}