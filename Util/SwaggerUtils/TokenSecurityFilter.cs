using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using ProjectAveryCommon.ExtensionMethods;
using ProjectAveryCommon.Model.Privileges;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectAvery.Util.SwaggerUtils;

// ReSharper disable once ClassNeverInstantiated.Global
// This is instantiated by Swashbuckle
public class TokenSecurityFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var privilegeAttributes = context.MethodInfo.CustomAttributes
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
                        new string[] { }
                    }
                });
            string privilegesString = string.Join(", ", privilegeAttributes.Select(a =>
            {
                var value = a.ConstructorArguments.FirstOrDefault().Value;
                if (value != null)
                    return ((Type)value).FriendlyName();
                return "Parse Error!";
            }));
            operation.Description = 
                $"<b>Required {(privilegeAttributes.Count > 1 ? "privileges" : "privilege")}:</b> {privilegesString}<br/>" + operation.Description;
        }
    }
}