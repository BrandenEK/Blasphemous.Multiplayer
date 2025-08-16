
namespace Blasphemous.Multiplayer.Server;

public class ServerSettings
{
    public int Port { get; set; } = 8989;

    public int MaxPlayers { get; set; } = 8;

    public string Password { get; set; } = string.Empty;
}
