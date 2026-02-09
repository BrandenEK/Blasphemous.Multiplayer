using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class PositionResponsePacket(string player, float x, float y) : BasePacket
{
    public string Player { get; set; } = player;

    public float X { get; set; } = x;

    public float Y { get; set; } = y;
}
