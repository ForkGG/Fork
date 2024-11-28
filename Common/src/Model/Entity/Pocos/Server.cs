using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ForkCommon.Model.Entity.Enums;
using ForkCommon.Model.Entity.Pocos.Automation;
using ForkCommon.Model.Entity.Pocos.Player;
using ForkCommon.Model.Entity.Pocos.ServerSettings;
using ForkCommon.Model.Entity.Transient.Console;
using Newtonsoft.Json;

namespace ForkCommon.Model.Entity.Pocos;

public class Server : IEntity
{
    public Server(string name, ServerVersion version, VanillaSettings vanillaSettings, JavaSettings javaSettings)
    {
        Name = name;
        Version = version;
        JavaSettings = javaSettings;
        VanillaSettings = vanillaSettings;
        AutomationTimes = new List<AutomationTime>(8)
        {
            new() { Enabled = false, Time = new SimpleTime(0, 0), Type = AutomationType.Restart },
            new() { Enabled = false, Time = new SimpleTime(6, 0), Type = AutomationType.Restart },
            new() { Enabled = false, Time = new SimpleTime(12, 0), Type = AutomationType.Restart },
            new() { Enabled = false, Time = new SimpleTime(18, 0), Type = AutomationType.Restart },
            new() { Enabled = false, Time = new SimpleTime(0, 0), Type = AutomationType.Stop },
            new() { Enabled = false, Time = new SimpleTime(12, 0), Type = AutomationType.Stop },
            new() { Enabled = false, Time = new SimpleTime(0, 0), Type = AutomationType.Start },
            new() { Enabled = false, Time = new SimpleTime(12, 0), Type = AutomationType.Start }
        };
    }

    /// <summary>
    ///     Constructor for deserializer/ef core
    ///     This is called, because the default constructor does contain unmapped values
    /// </summary>
    public Server()
    {
        // We need to load the server settings here
        //TODO CKE
    }

    public bool AutoSetSha1 { get; set; } = true;
    public DateTime? ResourcePackHashAge { get; set; } = DateTime.MinValue;

    public List<AutomationTime>? AutomationTimes { get; set; }

    public List<ServerPlayer> ServerPlayers { get; set; } = new();
    [NotMapped] public List<Player.Player> Whitelist { get; set; } = new();
    [NotMapped] public List<Player.Player> Banlist { get; set; } = new();

    [NotMapped] public VanillaSettings? VanillaSettings { get; set; }

    [NotMapped] [JsonIgnore] public string FullName => Name + " (" + Version?.Version + ")";

    [NotMapped] [JsonIgnore] public string? JarLink => Version?.JarLink;

    public ulong Id { get; set; }
    public string? Name { get; set; }
    public ServerVersion? Version { get; set; }
    public JavaSettings? JavaSettings { get; set; }

    public bool Initialized { get; set; } = false;
    public bool StartWithFork { get; set; } = false;
    public int ServerIconId { get; set; }

    [NotMapped] [JsonIgnore] public List<ConsoleMessage> ConsoleMessages { get; set; } = new();
    [NotMapped] public EntityStatus? Status { get; set; } = EntityStatus.Stopped;
    [NotMapped] [JsonIgnore] public Action<string>? ConsoleHandler { get; set; }

    public override string ToString()
    {
        string? name = Name;
        if (Name?.Length > 10)
        {
            name = name?.Substring(0, 10);
        }

        return name + " (" + Version?.Version + ")";
    }
}