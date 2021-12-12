using System.Collections.Generic;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectAvery.Util.SwaggerUtils;

public class TokenSecurityFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme, Id = "Fork Token"},
                        Name = "Fork Token",
                        In = ParameterLocation.Header,
                        BearerFormat = "Token"
                    },
                    new string[] { }
                }
            });
    }
}