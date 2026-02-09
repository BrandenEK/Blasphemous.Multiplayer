using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class IntroPacket(byte protocol, string room, string player, string password, byte team) : BasePacket
{
    public byte ProtocolVersion { get; set; } = protocol;

    public string RoomName { get; set; } = room;

    public string PlayerName { get; set; } = player;

    public string Password { get; set; } = password;

    public byte TeamNumber { get; set; } = team;
}
