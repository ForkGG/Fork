using System;
using System.ComponentModel.DataAnnotations;

namespace ForkCommon.Model.Entity.Pocos.Player;

public class Player : IComparable
{
    [Key] 
    public string Uid { get; set; }

    //This is set if the player is not found in the Mojang player API
    public bool IsOfflinePlayer { get; set; } = false;
    // Name of the player
    public string Name { get; set; }
    // Base 64 of the players Head
    public string Head { get; set; }
    // Last time the player got updated (Players are cached for a certain time in the database)
    public DateTime LastUpdated { get; set; } = DateTime.MinValue;

    
    public bool Equals(Player other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return IsOfflinePlayer == other.IsOfflinePlayer && Name == other.Name && Uid == other.Uid;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Player)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsOfflinePlayer, Name, Uid);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Player player)
            return String.Compare(Name, player.Name, StringComparison.Ordinal);
        throw new ArgumentException("Object is not a Player");
    }
}