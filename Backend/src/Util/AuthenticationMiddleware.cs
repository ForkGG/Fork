using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Fork.Logic.Services.AuthenticationServices;
using ForkCommon.Model.Privileges;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace Fork.Util;

public class AuthenticationMiddleware
{
    private readonly ILogger<AuthenticationMiddleware> _logger;
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(ILogger<AuthenticationMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, AuthenticationService authenticationService)
    {
        string? token = context.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(token))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        try
        {
            authenticationService.AuthenticateToken(token);
            ControllerActionDescriptor? metadata =
                context.GetEndpoint()!.Metadata.GetMetadata<ControllerActionDescriptor>();
            bool authenticated = metadata!.EndpointMetadata
                .All(a => a is not PrivilegesAttribute ||
                          (a is PrivilegesAttribute p && authenticationService.IsAuthenticated(p.Privilege)));
            if (!authenticated)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }
        catch (Exception e)
        {
            _logger.LogDebug($"Aborting request because of failed Authentication: {e.Message}");
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }

        await _next(context);
    }
}