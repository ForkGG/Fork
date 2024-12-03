using System;
using System.Collections.Generic;

namespace Fork.Adapters.Mojang;

public class Manifest
{
    public enum VersionType
    {
        release,
        snapshot,
        old_beta,
        old_alpha
    }

    public required Latest latest { get; set; }
    public required List<Version> versions { get; set; }

    public class Latest
    {
        public required string release { get; set; }
        public required string snapshot { get; set; }
    }

    public class Version
    {
        public required string id { get; set; }
        public required VersionType type { get; set; }
        public required string url { get; set; }
        public required DateTime time { get; set; }
        public required DateTime releaseTime { get; set; }
    }
}