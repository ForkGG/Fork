namespace Fork.Logic.Model.Web.Mojang;

public class PlayerTextureProfile
{
    public long? Timestamp { get; set; }
    public string? ProfileId { get; set; }
    public string? ProfileName { get; set; }
    public Texture? Textures { get; set; }

    public class Texture
    {
        public Skin? Skin { get; set; }
    }

    public class Skin
    {
        public string? Url { get; set; }
    }
}