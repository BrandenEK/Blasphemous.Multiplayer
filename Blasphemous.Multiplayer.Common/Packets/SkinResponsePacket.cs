using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class SkinResponsePacket(string player, string skin) : BasePacket
{
    public string Player { get; set; } = player;

    public string Skin { get; set; } = skin;
}
