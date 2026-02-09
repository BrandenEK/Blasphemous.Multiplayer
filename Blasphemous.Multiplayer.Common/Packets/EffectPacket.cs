using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class EffectPacket(byte type) : BasePacket
{
    public byte Type { get; set; } = type;
}
