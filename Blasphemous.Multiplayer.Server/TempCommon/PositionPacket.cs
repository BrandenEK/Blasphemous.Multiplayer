using Basalt.Framework.Networking;

namespace Blasphemous.Multiplayer.Server.TempCommon;

public class PositionPacket(float x, float y) : BasePacket
{
    public float X { get; } = x;

    public float Y { get; } = y;
}
