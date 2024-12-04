using System.Collections.Generic;

namespace ForkCommon.Model.Entity.Pocos.ServerSettings;

public abstract class AbstractSettings
{
    protected AbstractSettings(string name, Dictionary<string, string> settingsDictionary)
    {
        Name = name;
        SettingsDictionary = settingsDictionary;
    }

    public string Name { get; set; }

    public Dictionary<string, string> SettingsDictionary { get; protected set; }
}