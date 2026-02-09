using Basalt.Framework.Networking;
using Blasphemous.Multiplayer.Common.Enums;

namespace Blasphemous.Multiplayer.Common.Packets;

public class IntroResponsePacket(RefusalType type) : BasePacket
{
    public RefusalType Type { get; set; } = type;
}
