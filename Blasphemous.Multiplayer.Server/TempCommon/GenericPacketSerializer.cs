using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Serializers;
using System.Reflection;

namespace Blasphemous.Multiplayer.Server.TempCommon;

public class GenericPacketSerializer : IPacketSerializer
{
    public byte[] Serialize(BasePacket packet)
    {
        var properties = packet.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (var prop in properties)
        {
            Logger.Warn("Found prop " + prop.Name);
        }

        return new byte[] { 0 };
    }

    public BasePacket Deserialize(byte[] data)
    {
        throw new System.NotImplementedException();
    }
}
