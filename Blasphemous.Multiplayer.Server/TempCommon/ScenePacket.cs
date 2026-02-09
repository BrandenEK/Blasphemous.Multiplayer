using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Server.TempCommon;

public class ScenePacket(string scene) : BasePacket
{
    public string Scene { get; set; } = scene;
}
