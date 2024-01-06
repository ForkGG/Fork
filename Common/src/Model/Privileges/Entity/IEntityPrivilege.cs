namespace ForkCommon.Model.Privileges.Entity;

public interface IEntityPrivilege : IPrivilege
{
    public ulong EntityId { get; set; }
}