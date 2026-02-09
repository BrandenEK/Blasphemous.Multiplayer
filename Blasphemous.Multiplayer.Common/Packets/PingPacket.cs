using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class PingPacket(float time, ushort ping) : BasePacket
{
    public float TimeStamp { get; set; } = time;

    public ushort Ping { get; set; } = ping;
}
