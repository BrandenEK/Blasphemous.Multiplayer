using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class EffectResponsePacket(string player, byte type) : BasePacket
{
    public string Player { get; set; } = player;

    public byte Type { get; set; } = type;
}
