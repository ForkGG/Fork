using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Enums.Player;
using ProjectAveryCommon.Model.Entity.Pocos;
using ProjectAveryCommon.Model.Entity.Pocos.Player;

namespace ProjectAvery.Logic.Managers;

public interface IEntityManager
{
    public Task<IEntity> EntityById(ulong entityId);
    public Task<List<IEntity>> ListAllEntities();

    public Task UpdatePlayerOnPlayerList(Server server, ServerPlayer player);
    public Task UpdatePlayerOnWhitelist(Server server, Player player, PlayerlistUpdateType updateType);
    public Task UpdatePlayerOnBanList(Server server, Player player, PlayerlistUpdateType updateType);
}