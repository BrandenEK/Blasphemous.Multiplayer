
namespace Blasphemous.Multiplayer.Server;

public class ServerSettings
{
    public int Port { get; }

    public int MaxPlayers { get; }

    public string Password { get; }

    public ServerSettings(int port, int maxPlayers, string password)
    {
        Port = port;
        MaxPlayers = maxPlayers;
        Password = password;
    }
}
