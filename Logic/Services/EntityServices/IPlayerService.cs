using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos.Player;

namespace ProjectAvery.Logic.Services.EntityServices;

/// <summary>
/// Service to get player information from the Mojang servers
/// </summary>
public interface IPlayerService
{
    /// <summary>
    /// Gets a player by the name of that player
    /// This is less efficient than by UID
    /// </summary>
    public Task<Player> PlayerByNameAsync(string name);

    /// <summary>
    /// Gets a player by the UID of that player
    /// This is more efficient than by name
    /// </summary>
    public Task<Player> PlayerByUidAsync(string uid);

    public Task<ISet<string>> PlayerUidsForWorldsAsync(List<string> worldPaths);
}