using ForkCommon.Model.Entity.Pocos.Player;

namespace ForkFrontend.Shared.Components.Entity.Tabs.ConsoleTab.Playerlist;

public interface ISimplePlayerlist
{
    public Player? ActivePlayer { get; set; }
    public void SelectPlayer(PlayerComponent playerComponent);
}