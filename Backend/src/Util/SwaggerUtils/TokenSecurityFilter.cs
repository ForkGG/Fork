using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ForkCommon.ExtensionMethods;
using ForkCommon.Model.Privileges;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fork.Util.SwaggerUtils;

// ReSharper disable once ClassNeverInstantiated.Global
// This is instantiated by Swashbuckle
public class TokenSecurityFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        List<CustomAttributeData> privilegeAttributes = context.MethodInfo.CustomAttributes
            .Where(a => a.AttributeType == typeof(PrivilegesAttribute))
            .Concat(context.MethodInfo.DeclaringType?.CustomAttributes.Where(a =>
                a.AttributeType == typeof(PrivilegesAttribute)) ?? Array.Empty<CustomAttributeData>()).ToList();

        if (privilegeAttributes.Any())
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Fork Token" },
                            Name = "Fork Token",
                            In = ParameterLocation.Header,
                            BearerFormat = "Token"
                        },
                        []
                    }
                });
            string privilegesString = string.Join(", ", privilegeAttributes.Select(a =>
            {
                object? value = a.ConstructorArguments.FirstOrDefault().Value;
                return value != null ? ((Type)value).FriendlyName() : "Parse Error!";
            }));
            operation.Description =
                $"<b>Required {(privilegeAttributes.Count > 1 ? "privileges" : "privilege")}:</b> {privilegesString}<br/>" +
                operation.Description;
        }
    }
}