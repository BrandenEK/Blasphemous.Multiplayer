using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class PositionPacket(float x, float y) : BasePacket
{
    public float X { get; set; } = x;

    public float Y { get; set; } = y;
}
