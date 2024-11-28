namespace ForkCommon.Model.Privileges;

public class PrivilegeDescriptor<T> where T : IPrivilege
{
    public PrivilegeDescriptor(T type)
    {
        Type = type;
    }

    public T Type { get; set; }
}