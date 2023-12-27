using Microsoft.AspNetCore.Builder;

namespace Fork.Util.ExtensionMethods;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}