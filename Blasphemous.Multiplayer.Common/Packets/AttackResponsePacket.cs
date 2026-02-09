using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class AttackResponsePacket(string player, byte type, byte amount, string victim) : BasePacket
{
    public string Player { get; set; } = player;

    public byte Type { get; set; } = type;

    public byte Amount { get; set; } = amount;

    public string Victim { get; set; } = victim;
}
