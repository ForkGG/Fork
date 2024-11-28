using System;
using System.ComponentModel.DataAnnotations;

namespace ForkCommon.Model.Entity.Pocos.Player;

public class Player : IComparable
{
    public Player(string uid)
    {
        Uid = uid;
    }

    [Key] public string Uid { get; set; }

    //This is set if the player is not found in the Mojang player API
    public bool IsOfflinePlayer { get; set; } = false;

    // Name of the player
    public string? Name { get; set; }

    // Base 64 of the players Head
    public string? Head { get; set; }

    // Last time the player got updated (Players are cached for a certain time in the database)
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;

    public int CompareTo(object? obj)
    {
        if (obj is Player player)
        {
            return string.Compare(Name, player.Name, StringComparison.Ordinal);
        }

        throw new ArgumentException("Object is not a Player");
    }


    protected bool Equals(Player other)
    {
        return Uid == other.Uid && IsOfflinePlayer == other.IsOfflinePlayer && Name == other.Name &&
               Head == other.Head && LastUpdated.Equals(other.LastUpdated);
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

        return Equals((Player)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Uid, IsOfflinePlayer, Name, Head, LastUpdated);
    }
}