using Newtonsoft.Json;
using ForkCommon.Model.Entity.Enums;

namespace ForkCommon.Model.Entity.Pocos.Automation
{
    /// <summary>
    /// A class for all automations
    /// </summary>
    public class AutomationTime
    {
        public ulong Id { get; set; }
        public bool Enabled { get; set; }
        public AutomationType Type { get; set; }
        public SimpleTime Time { get; set; } 
        
        public ulong ServerId { get; set; }
        public Server Server { get; set; }
    }
}