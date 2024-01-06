using System.ComponentModel;

namespace ForkCommon.Model.Privileges;

[Description("Any privilege")]
public interface IPrivilege
{
    public string Name { get; }
    public string TranslationPath { get; }
}