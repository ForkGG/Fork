using System.Threading.Tasks;
using Fork.Logic.Managers;
using ForkCommon.Model.Application;

namespace Fork.Logic.Services.StateServices;

public class ApplicationStateService
{
    private readonly ApplicationManager _applicationManager;
    private readonly EntityManager _entityManager;

    public ApplicationStateService(ApplicationManager applicationManager, EntityManager entityManager)
    {
        _applicationManager = applicationManager;
        _entityManager = entityManager;
    }

    //TODO CKE check permission before adding stuff
    public async Task<State> BuildAppState()
    {
        State result = new(await _entityManager.ListAllEntities());
        return result;
    }
}