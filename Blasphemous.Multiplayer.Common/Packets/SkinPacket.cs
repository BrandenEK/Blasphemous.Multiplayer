using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class SkinPacket(string skin) : BasePacket
{
    public string Skin { get; set; } = skin;
}
