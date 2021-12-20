using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectAveryCommon.ExtensionMethods;
using ProjectAveryCommon.Model.Application.Exceptions;

namespace ProjectAvery.Util;

public class ForkExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override async Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is ForkException exception)
        {
            await context.HttpContext.Response.WriteAsync(exception.ToJson());
            context.ExceptionHandled = true;
        }
        await base.OnExceptionAsync(context);
    }
}