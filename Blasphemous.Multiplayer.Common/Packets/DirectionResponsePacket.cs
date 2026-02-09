using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class DirectionResponsePacket(string player, bool direction) : BasePacket
{
    public string Player { get; set; } = player;

    public bool Direction { get; set; } = direction;
}
