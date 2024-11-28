namespace ForkCommon.Model.Entity.Pocos;

public class JavaVersion
{
    public ulong Id { get; set; }
    public string? Version { get; set; }
    public int VersionComputed { get; set; }
    public bool Is64Bit { get; set; }
}