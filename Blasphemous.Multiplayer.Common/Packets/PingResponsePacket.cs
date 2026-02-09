using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class PingResponsePacket(float time) : BasePacket
{
    public float Time { get; set; } = time;

    // Other players
}
