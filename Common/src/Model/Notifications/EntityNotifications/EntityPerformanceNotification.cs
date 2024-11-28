using System;

namespace ForkCommon.Model.Notifications.EntityNotifications;

public class EntityPerformanceNotification : AbstractEntityNotification
{
    public EntityPerformanceNotification(ulong entityId, double cpuPercentage, double ramPercentage, TimeSpan uptime) :
        base(entityId)
    {
        CpuPercentage = cpuPercentage;
        RamPercentage = ramPercentage;
        Uptime = uptime;
    }

    public double CpuPercentage { get; set; }
    public double RamPercentage { get; set; }
    public TimeSpan Uptime { get; set; }
}