using System.Collections.Generic;
using ProjectAveryCommon.Model.Entity.Pocos.Player;

namespace ProjectAvery.Logic;

public class ObjectCache : IObjectCache
{
    public Dictionary<string, Player> PlayersByUid { get; set; }
}