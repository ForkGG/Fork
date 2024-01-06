namespace ForkCommon.Model.Privileges.Entity.WriteEntity.WriteConsoleTab;

public class WriteConsoleTabPrivilege : IWriteEntityPrivilege
{
    public WriteConsoleTabPrivilege(ulong entityId)
    {
        EntityId = entityId;
    }

    public string Name => "WriteConsoleTab";
    public string TranslationPath => "privileges.entity.write.consoleTab";
    public ulong EntityId { get; set; }
}