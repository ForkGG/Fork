using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fork.Logic.Managers;
using Fork.Logic.Notification;
using Fork.Logic.Services.AuthenticationServices;
using Fork.Logic.Services.FileServices;
using Fork.Util.ExtensionMethods;
using ForkCommon.Model.Entity.Pocos;
using ForkCommon.Model.Entity.Pocos.Settings;
using ForkCommon.Model.Notifications.EntityNotifications;
using ForkCommon.Model.Privileges.Entity.ReadEntity.ReadSettingsTab;

namespace Fork.Logic.Services.EntityServices;

public class EntitySettingsService(
    AuthenticationService authenticationService,
    EntityManager entityManager,
    ApplicationManager applicationManager,
    FileWriterService fileWriter,
    INotificationCenter notificationCenter)
{
    private static readonly ConcurrentDictionary<ulong, SemaphoreSlim> _writeLocks = new();

    public async Task<List<AbstractSettings>> GetAllSettingsForEntity(ulong entityId)
    {
        IEntity? entity = await entityManager.EntityById(entityId);
        if (entity == null)
        {
            return [];
        }

        List<AbstractSettings> result = [];
        result.Add(entity.EntitySettings);
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

    public async Task UpdateVanillaSettingsAsync(ulong entityId, VanillaSettings settings)
    {
        IEntity? entity = await entityManager.EntityById(entityId);
        if (entity is not Server server) return;

        server.VanillaSettings = settings;

        SemaphoreSlim writeLock = _writeLocks.GetOrAdd(entityId, _ => new SemaphoreSlim(1, 1));
        await writeLock.WaitAsync();
        try
        {
            await fileWriter.WriteServerSettings(entity.GetPath(applicationManager), settings.SettingsDictionary);
        }
        finally
        {
            writeLock.Release();
        }

        await notificationCenter.BroadcastNotification(new VanillaSettingsChangedNotification(entityId, settings));
    }
}
