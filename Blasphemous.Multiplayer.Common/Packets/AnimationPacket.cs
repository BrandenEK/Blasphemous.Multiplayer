using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class AnimationPacket(byte animation) : BasePacket
{
    public byte Animation { get; set; } = animation;
}
