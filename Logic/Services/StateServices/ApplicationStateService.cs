using System.Threading.Tasks;
using ProjectAvery.Logic.Managers;
using ProjectAveryCommon.Model.Application;

namespace ProjectAvery.Logic.Services.StateServices;

public class ApplicationStateService : IApplicationStateService
{
    private readonly IApplicationManager _applicationManager;
    private readonly IEntityManager _entityManager;

    public ApplicationStateService(IApplicationManager applicationManager, IEntityManager entityManager)
    {
        _applicationManager = applicationManager;
        _entityManager = entityManager;
    }

    //TODO CKE check permission before adding stuff
    public async Task<State> BuildAppState()
    {
        State result = new State();
        result.Entities = await _entityManager.ListAllEntities();
        return result;
    }
}