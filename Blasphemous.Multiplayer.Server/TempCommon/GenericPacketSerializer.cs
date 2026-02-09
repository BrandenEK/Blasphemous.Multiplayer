using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Serializers;
using Basalt.Framework.Networking.Streams;
using System;
using System.Linq;
using System.Reflection;

namespace Blasphemous.Multiplayer.Server.TempCommon;

public class GenericPacketSerializer : IPacketSerializer
{
    private readonly Func<BasePacket> _creator;

    public GenericPacketSerializer(Func<BasePacket> creator)
    {
        _creator = creator;
    }

    public byte[] Serialize(BasePacket packet)
    {
        var properties = packet.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .OrderBy(p => p.Name);

        var stream = new OutStream();

        foreach (var p in properties)
        {
            TypeCode type = Type.GetTypeCode(p.PropertyType);

            switch (type)
            {
                case TypeCode.Byte:
                    stream.Write_byte((byte)p.GetValue(packet));
                    break;
                case TypeCode.SByte:
                    stream.Write_sbyte((sbyte)p.GetValue(packet));
                    break;
                case TypeCode.UInt16:
                    stream.Write_ushort((ushort)p.GetValue(packet));
                    break;
                case TypeCode.Int16:
                    stream.Write_short((short)p.GetValue(packet));
                    break;
                case TypeCode.UInt32:
                    stream.Write_uint((uint)p.GetValue(packet));
                    break;
                case TypeCode.Int32:
                    stream.Write_int((int)p.GetValue(packet));
                    break;
                case TypeCode.UInt64:
                    stream.Write_ulong((ulong)p.GetValue(packet));
                    break;
                case TypeCode.Int64:
                    stream.Write_long((long)p.GetValue(packet));
                    break;
                case TypeCode.Single:
                    stream.Write_float((float)p.GetValue(packet));
                    break;
                case TypeCode.Double:
                    stream.Write_double((double)p.GetValue(packet));
                    break;
                case TypeCode.Boolean:
                    stream.Write_bool((bool)p.GetValue(packet));
                    break;
                case TypeCode.Char:
                    stream.Write_char((char)p.GetValue(packet));
                    break;
                case TypeCode.String:
                    stream.Write_string((string)p.GetValue(packet));
                    break;
                default:
                    throw new Exception($"Can not serialize a property of type {type}");
            }
        }

        return stream;
    }

    public BasePacket Deserialize(byte[] data)
    {
        var packet = _creator();

        var properties = packet.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .OrderBy(p => p.Name);

        var stream = new InStream(data);

        foreach (var p in properties)
        {
            TypeCode type = Type.GetTypeCode(p.PropertyType);

            switch (type)
            {
                case TypeCode.Byte:
                    p.SetValue(packet, stream.Read_byte());
                    break;
                case TypeCode.SByte:
                    p.SetValue(packet, stream.Read_sbyte());
                    break;
                case TypeCode.UInt16:
                    p.SetValue(packet, stream.Read_ushort());
                    break;
                case TypeCode.Int16:
                    p.SetValue(packet, stream.Read_short());
                    break;
                case TypeCode.UInt32:
                    p.SetValue(packet, stream.Read_uint());
                    break;
                case TypeCode.Int32:
                    p.SetValue(packet, stream.Read_int());
                    break;
                case TypeCode.UInt64:
                    p.SetValue(packet, stream.Read_ulong());
                    break;
                case TypeCode.Int64:
                    p.SetValue(packet, stream.Read_long());
                    break;
                case TypeCode.Single:
                    p.SetValue(packet, stream.Read_float());
                    break;
                case TypeCode.Double:
                    p.SetValue(packet, stream.Read_double());
                    break;
                case TypeCode.Boolean:
                    p.SetValue(packet, stream.Read_bool());
                    break;
                case TypeCode.Char:
                    p.SetValue(packet, stream.Read_char());
                    break;
                case TypeCode.String:
                    p.SetValue(packet, stream.Read_string());
                    break;
                default:
                    throw new Exception($"Can not deserialize a property of type {type}");
            }
        }

        return packet;
    }
}
