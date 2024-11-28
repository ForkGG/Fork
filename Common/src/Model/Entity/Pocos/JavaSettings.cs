namespace ForkCommon.Model.Entity.Pocos;

public class JavaSettings
{
    public ulong Id { get; set; }
    public int MaxRam { get; set; } = 2048;
    public string? JavaPath { get; set; }
    public string? StartupParameters { get; set; }
}