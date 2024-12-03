using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using ForkCommon.Model.Entity.Enums;

namespace ForkCommon.Model.Entity.Pocos;

public class ServerVersion
{
    public static readonly ServerVersion Version1_13 = new()
    {
        Version = "1.13"
    };

    public static readonly ServerVersion Version1_18 = new()
    {
        Version = "1.18"
    };

    private readonly Regex _nonNumeric = new(@"[^\d.]");

    public ulong Id { get; set; }
    public VersionType Type { get; set; }
    public string? Version { get; set; }
    public int Build { get; set; } = 0;

    /**
     * JarLink to the server.jar file
     * <br />
     * WARNING:
     * - For Vanilla/VanillaSnapshot this is just a link to the json containing the download URL (i.e. https://piston-meta.mojang.com/v1/packages/a3bcba436caa849622fd7e1e5b89489ed6c9ac63/1.21.4.json)
     * - For Paper Versions this is not set, but needs to be requested when downloading (depending on build number)
     */
    public string? JarLink { get; set; }

    [NotMapped] public bool IsProxy => Type == VersionType.Waterfall;

    [NotMapped] public bool SupportBuilds => Type == VersionType.Paper;

    [NotMapped] public bool HasPlugins => Type is VersionType.Paper or VersionType.Spigot;

    public override string ToString()
    {
        return Version ?? "";
    }

    public ServerVersion Clone()
    {
        return MemberwiseClone() as ServerVersion ?? throw new InvalidOperationException();
    }

    public int CompareTo(object? obj)
    {
        if (obj == null)
        {
            return 1;
        }

        if (obj is ServerVersion otherVersion && Version != null && otherVersion.Version != null)
        {
            string friendlyVersion = _nonNumeric.Replace(Version, "");
            List<string> thisVersionSub = new(friendlyVersion.Split('.'));
            Version.Split('.');
            thisVersionSub.Add("0");
            thisVersionSub.Add("0");
            string friendlyOtherVersion = _nonNumeric.Replace(otherVersion.Version, "");
            List<string> otherVersionSub = new(friendlyOtherVersion.Split('.'));
            otherVersion.Version.Split('.');
            otherVersionSub.Add("0");
            otherVersionSub.Add("0");

            if (int.Parse(thisVersionSub[0]) < int.Parse(otherVersionSub[0]))
            {
                return -1;
            }

            if (int.Parse(thisVersionSub[0]) > int.Parse(otherVersionSub[0]))
            {
                return 1;
            }

            if (int.Parse(thisVersionSub[1]) < int.Parse(otherVersionSub[1]))
            {
                return -1;
            }

            if (int.Parse(thisVersionSub[1]) > int.Parse(otherVersionSub[1]))
            {
                return 1;
            }

            if (int.Parse(thisVersionSub[2]) < int.Parse(otherVersionSub[2]))
            {
                return -1;
            }

            if (int.Parse(thisVersionSub[2]) > int.Parse(otherVersionSub[2]))
            {
                return 1;
            }

            return 0;
        }

        throw new ArgumentException("Object is not a Minecraft Version");
    }

    public bool IsEqualOrGreaterThan(ServerVersion other)
    {
        return CompareTo(other) > 0;
    }

    protected bool Equals(ServerVersion other)
    {
        return Type == other.Type && Version == other.Version && JarLink == other.JarLink;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is ServerVersion otherVersion)
        {
            return Equals(otherVersion);
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = (int)Type;
            hashCode = (hashCode * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (JarLink != null ? JarLink.GetHashCode() : 0);
            return hashCode;
        }
    }
}