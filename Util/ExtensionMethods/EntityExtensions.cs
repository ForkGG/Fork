using System;
using System.Diagnostics;
using System.IO;
using ProjectAvery.Logic.Managers;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Util.ExtensionMethods;

public static class EntityExtensions
{
    public static string GetPath(this IEntity entity, IApplicationManager applicationManager)
    {
        return Path.Combine(applicationManager.EntityPath, entity.Name.Trim().Replace(" ", ""));
    }
}