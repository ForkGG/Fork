using System.Collections.Generic;

namespace Fork.Logic.Model.Web.Mojang;

public class PlayerProfile
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public List<PlayerProperty>? Properties { get; set; }
}