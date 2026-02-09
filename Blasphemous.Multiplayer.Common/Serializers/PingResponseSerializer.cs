using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Serializers;
using Basalt.Framework.Networking.Streams;
using Blasphemous.Multiplayer.Common.Packets;
using System.Collections.Generic;

namespace Blasphemous.Multiplayer.Common.Serializers;

public class PingResponseSerializer : IPacketSerializer
{
    public byte[] Serialize(BasePacket packet)
    {
        PingResponsePacket p = (PingResponsePacket)packet;

        var stream = new OutStream();
        stream.Write_float(p.Time);

        foreach (var kvp in p.Pings)
        {
            stream.Write_string(kvp.Key);
            stream.Write_ushort(kvp.Value);
        }

        return stream;
    }

    public BasePacket Deserialize(byte[] data)
    {
        var stream = new InStream(data);

        float time = stream.Read_float();
        var pings = new Dictionary<string, ushort>();

        while (stream.Remaining > 2)
        {
            string name = stream.Read_string();
            ushort ping = stream.Read_ushort();
            pings.Add(name, ping);
        }

        return new PingResponsePacket(time, pings);
    }
}
