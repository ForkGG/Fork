using System.Threading.Tasks;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fork.Util;

public class ForkExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is ForkException exception)
        {
            context.HttpContext.Response.StatusCode = 500;
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync(exception.ToJson());
            context.ExceptionHandled = true;
        }

        await base.OnExceptionAsync(context);
    }
}