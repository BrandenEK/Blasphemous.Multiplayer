using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class SkinResponsePacket(string player, byte[] texture) : BasePacket
{
    public string Player { get; set; } = player;

    public byte[] Texture { get; set; } = texture;
}
