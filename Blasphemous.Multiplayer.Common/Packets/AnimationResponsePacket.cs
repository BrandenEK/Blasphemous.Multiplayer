using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class AnimationResponsePacket(string player, byte animation) : BasePacket
{
    public string Player { get; set; } = player;

    public byte Animation { get; set; } = animation;
}
