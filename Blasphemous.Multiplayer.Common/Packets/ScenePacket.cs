using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Common.Packets;

public class ScenePacket(string scene) : BasePacket
{
    public string Scene { get; set; } = scene;
}
