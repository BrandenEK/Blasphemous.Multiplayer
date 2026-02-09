using Basalt.Framework.Networking;
using System.Collections.Generic;

namespace Blasphemous.Multiplayer.Common.Packets;

public class PingResponsePacket(float time, Dictionary<string, ushort> pings) : BasePacket
{
    public float Time { get; set; } = time;

    public Dictionary<string, ushort> Pings = pings;
}
