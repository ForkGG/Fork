using System.Collections.Generic;

namespace ForkCommon.Model.Entity.Pocos.ServerSettings
{
    public abstract class AbstractSettings
    {
        public Dictionary<string, string> SettingsDictionary { get; protected set; }
    }
}