using Basalt.Framework.Networking;
using Blasphemous.Multiplayer.Common.Enums;

namespace Blasphemous.Multiplayer.Common.Packets;

public class ProgressPacket(ProgressType type, string id, byte value) : BasePacket
{
    public ProgressType Type { get; set; } = type;

    public string Id { get; set; } = id;

    public byte Value { get; set; } = value;
}
