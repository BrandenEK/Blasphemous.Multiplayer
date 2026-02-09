using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Serializers;
using Basalt.Framework.Networking.Streams;
using System;
using System.Linq;
using System.Reflection;

namespace Blasphemous.Multiplayer.Server.TempCommon;

public class GenericPacketSerializer : IPacketSerializer
{
    private readonly Type _packetType;

    public GenericPacketSerializer(Type packetType)
    {
        _packetType = packetType;
    }

    public byte[] Serialize(BasePacket packet)
    {
        var properties = packet.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .OrderBy(p => p.Name);

        foreach (var prop in properties)
        {
            Logger.Warn($"Found prop {prop.Name}: {prop.GetValue(packet)}");
        }

        var stream = new OutStream();

        foreach (var p in properties)
        {
            switch (Type.GetTypeCode(p.PropertyType))
            {
                case TypeCode.Single:
                    stream.Write_float((float)p.GetValue(packet));
                    break;
            }
        }

        foreach (byte b in (byte[])stream)
        {
            Logger.Error(b);
        }

        return stream;
    }

    public BasePacket Deserialize(byte[] data)
    {
        //var packet = new PositionPacket(0, 0);
        var packet = (BasePacket)Activator.CreateInstance(_packetType);

        var stream = new InStream(data);

        var properties = packet.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .OrderBy(p => p.Name);

        foreach (var prop in properties)
        {
            Logger.Warn($"Found prop {prop.Name}: {prop.GetValue(packet)}");
        }

        foreach (var p in properties)
        {
            switch (Type.GetTypeCode(p.PropertyType))
            {
                case TypeCode.Single:
                    p.SetValue(packet, stream.Read_float());
                    break;
            }
        }

        foreach (var prop in properties)
        {
            Logger.Warn($"Found prop {prop.Name}: {prop.GetValue(packet)}");
        }

        //packet.X = stream.Read_float();
        //packet.Y = stream.Read_float();

        return packet;
    }
}
