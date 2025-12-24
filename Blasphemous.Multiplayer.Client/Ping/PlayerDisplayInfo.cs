
namespace Blasphemous.Multiplayer.Client.Ping;

internal class PlayerDisplayInfo(string name, byte team, ushort ping)
{
    public string Name { get; } = name;

    public byte Team { get; } = team;

    public ushort Ping { get; } = ping;
}
