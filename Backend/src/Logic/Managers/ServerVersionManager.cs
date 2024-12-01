using System.Collections.Generic;
using ForkCommon.Model.Entity.Enums;

namespace Fork.Logic.Managers;

public class ServerVersionManager
{
    public readonly List<VersionType> SupportedVersionTypes = new() { VersionType.Vanilla, VersionType.Paper };
}