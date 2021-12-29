using System.Collections.Generic;
using ForkCommon.Model.Entity.Pocos.Player;

namespace Fork.Logic;

public class ObjectCache : IObjectCache
{
    public Dictionary<string, Player> PlayersByUid { get; set; }
}