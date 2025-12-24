
namespace Blasphemous.Multiplayer.Client.Ping;

internal class PingRegion(int maxPing, string color)
{
    public int MaxPing { get; } = maxPing;

    public string Color { get; } = color;
}
