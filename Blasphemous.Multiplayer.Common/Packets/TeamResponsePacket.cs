using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class TeamResponsePacket(string player, byte team) : BasePacket
{
    public string Player { get; set; } = player;

    public byte Team { get; set; } = team;
}
