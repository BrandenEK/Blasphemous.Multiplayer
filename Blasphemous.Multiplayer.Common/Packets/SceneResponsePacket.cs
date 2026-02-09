using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class SceneResponsePacket(string player, string scene) : BasePacket
{
    public string Player { get; set; } = player;

    public string Scene { get; set; } = scene;
}
