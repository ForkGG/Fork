using System;

namespace ForkCommon.Model.Notifications.EntityNotifications;

public class EntityPerformanceNotification : AbstractEntityNotification
{
    public double CpuPercentage { get; set; }
    public double RamPercentage { get; set; }
    public TimeSpan Uptime { get; set; }
}