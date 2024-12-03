using System.Threading.Tasks;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Fork.Util;

public class ForkExceptionFilterAttribute(ILogger<ForkExceptionFilterAttribute> logger) : ExceptionFilterAttribute
{
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is ForkException exception)
        {
            logger.LogError(exception, "Unhandled exception occured");

            context.HttpContext.Response.StatusCode = 500;
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(exception.ToJson());
            context.ExceptionHandled = true;
        }

        await base.OnExceptionAsync(context);
    }
}