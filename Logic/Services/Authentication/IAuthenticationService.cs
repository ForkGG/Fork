using ProjectAvery.Logic.Model.Enums;

namespace ProjectAvery.Logic.Services.Authentication;

/// <summary>
/// A scoped service interface for handling authentication with tokens
/// </summary>
public interface IAuthenticationService
{
    void AuthenticateToken(string token);
    bool IsAuthenticated(Privilege privilege);
}