using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class SkinPacket(string name) : BasePacket
{
    public string Name { get; set; } = name;
}
