
namespace Blasphemous.Multiplayer.Server;

public class ServerSettings
{
    public int Port { get; } = 8989;

    public int MaxPlayers { get; } = 8;

    public string Password { get; } = string.Empty;

    public ServerSettings(int port, int maxPlayers, string password)
    {
        Port = port;
        MaxPlayers = maxPlayers;
        Password = password;
    }
}
