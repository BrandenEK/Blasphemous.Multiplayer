using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class TeamPacket(byte team) : BasePacket
{
    public byte Team { get; set; } = team;
}
