using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ForkCommon.Model.Entity.Pocos.Player;

/// <summary>
///     A player can exist on multiple servers
///     This instance stores server bases information
/// </summary>
public class ServerPlayer : IComparable
{
    public ServerPlayer(Player player, Server server)
    {
        Player = player;
        PlayerId = player.Uid;
        Server = server;
        ServerId = server.Id;
    }

    // EF Constructor
    [JsonConstructor]
    private ServerPlayer()
    {
    }

    [Key] public ulong Id { get; set; }

    // The player this instance is relating to
    public Player Player { get; set; } = default!;

    [ForeignKey(nameof(Player))] public string PlayerId { get; set; } = default!;

    // The server this instance is relating to
    [JsonIgnore] public Server? Server { get; set; }

    public ulong ServerId { get; set; }

    [NotMapped] public bool IsOp { get; set; }

    [NotMapped] public bool IsOnline { get; set; }

    public int CompareTo(object? obj)
    {
        if (obj is ServerPlayer serverPlayer)
        {
            return CompareTo(serverPlayer);
        }

        throw new ArgumentException("Object is not a ServerPlayer");
    }

    public int CompareTo(ServerPlayer other)
    {
        int onlineCompare = other.IsOnline.CompareTo(IsOnline);
        if (onlineCompare != 0)
        {
            return onlineCompare;
        }

        int opCompare = other.IsOp.CompareTo(IsOp);
        if (opCompare != 0)
        {
            return opCompare;
        }

        return string.Compare(Player.Name, other.Player.Name, StringComparison.Ordinal);
    }

    protected bool Equals(ServerPlayer other)
    {
        return Id == other.Id && Player.Equals(other.Player) && PlayerId == other.PlayerId &&
               ServerId == other.ServerId && IsOp == other.IsOp && IsOnline == other.IsOnline;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ServerPlayer)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Player, PlayerId, Server, ServerId, IsOp, IsOnline);
    }
}