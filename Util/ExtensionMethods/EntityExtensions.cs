using System;
using System.Diagnostics;
using System.IO;
using Fork.Logic.Managers;
using ForkCommon.Model.Entity.Pocos;

namespace Fork.Util.ExtensionMethods;

public static class EntityExtensions
{
    public static string GetPath(this IEntity entity, IApplicationManager applicationManager)
    {
        return Path.Combine(applicationManager.EntityPath, entity.Name.Trim().Replace(" ", ""));
    }
}