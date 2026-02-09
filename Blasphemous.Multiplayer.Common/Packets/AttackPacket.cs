using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class AttackPacket(byte type, byte amount, string player) : BasePacket
{
    public byte Type { get; set; } = type;

    public byte Amount { get; set; } = amount;

    public string Player { get; set; } = player;
}
