namespace Fork.Logic.Model.MinecraftVersionModels;

public class VersionDetails
{
    public required Downloads downloads { get; set; }

    public class Client
    {
        public required string sha1 { get; set; }
        public required int size { get; set; }
        public required string url { get; set; }
    }

    public class Server
    {
        public required string sha1 { get; set; }
        public required int size { get; set; }
        public required string url { get; set; }
    }

    public class Downloads
    {
        public required Client client { get; set; }
        public required Server server { get; set; }
    }
}