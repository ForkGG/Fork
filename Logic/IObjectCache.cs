using System.Collections.Generic;
using ForkCommon.Model.Entity.Pocos.Player;

namespace Fork.Logic;

/// <summary>
/// Basic Singleton that can be injected to store cached objects
/// </summary>
public interface IObjectCache
{
    /// <summary>
    /// Cache players from the Mojang API here to reduce network load
    /// Only players that were found in the API are stored. Offline players are checked each time
    /// </summary>
    public Dictionary<string, Player> PlayersByUid { get; set; }
}