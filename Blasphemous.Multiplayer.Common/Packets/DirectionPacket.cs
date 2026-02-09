using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class DirectionPacket(bool direction) : BasePacket
{
    public bool Direction { get; set; } = direction;
}
