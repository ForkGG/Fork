using System.Collections.Generic;
using System.Threading.Tasks;
using ForkCommon.Model.Entity.Enums.Player;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Player;

namespace Fork.Logic.Managers;

public interface IEntityManager
{
    public Task<IEntity> EntityById(ulong entityId);
    public Task<List<IEntity>> ListAllEntities();

    public Task UpdatePlayerOnPlayerList(Server server, ServerPlayer player);
    public Task UpdatePlayerOnWhitelist(Server server, Player player, PlayerlistUpdateType updateType);
    public Task UpdatePlayerOnBanList(Server server, Player player, PlayerlistUpdateType updateType);
}