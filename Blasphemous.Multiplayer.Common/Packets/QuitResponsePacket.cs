using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class QuitResponsePacket(string player) : BasePacket
{
    public string Player { get; set; } = player;
}
