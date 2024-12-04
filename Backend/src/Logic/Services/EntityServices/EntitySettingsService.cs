using System.Collections.Generic;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using Fork.Logic.Services.AuthenticationServices;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.ServerSettings;
using ForkCommon.Model.Privileges.Entity.ReadEntity.ReadSettingsTab;

namespace Fork.Logic.Services.EntityServices;

public class EntitySettingsService(AuthenticationService authenticationService, EntityManager entityManager)
{
    public async Task<List<AbstractSettings>> GetAllSettingsForEntity(ulong entityId)
    {
        IEntity? entity = await entityManager.EntityById(entityId);
        if (entity == null)
        {
            return [];
        }

        List<AbstractSettings> result = [];
        if (entity is Server { VanillaSettings: not null } server &&
            authenticationService.IsAuthenticated(typeof(ReadVanillaSettingsTabPrivilege)))
        {
            result.Add(server.VanillaSettings);
        }

        if (authenticationService.IsAuthenticated(typeof(ReadVersionSpecificSettingsTabPrivilege)))
        {
            // TODO CKE load other settings
        }

        return result;
    }
}