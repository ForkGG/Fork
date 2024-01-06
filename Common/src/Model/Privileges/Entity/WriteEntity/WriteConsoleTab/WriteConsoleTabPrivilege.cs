namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteConsoleTab;

public class WriteConsoleTabPrivilege : IWriteEntityPrivilege
{
    public string Name => "WriteConsoleTab";
    public string TranslationPath => "privileges.entity.write.consoleTab";
    public ulong EntityId { get; set; }

    public WriteConsoleTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }
}