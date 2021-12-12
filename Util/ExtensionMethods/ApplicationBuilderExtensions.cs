using Microsoft.AspNetCore.Builder;

namespace ProjectAvery.Util.ExtensionMethods;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}