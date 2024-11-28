using System.Collections.Generic;

namespace ForkCommon.Model.Entity.Pocos.ServerSettings;

public abstract class AbstractSettings
{
    protected AbstractSettings(Dictionary<string, string> settingsDictionary)
    {
        SettingsDictionary = settingsDictionary;
    }

    public Dictionary<string, string> SettingsDictionary { get; protected set; }
}