
namespace Blasphemous.Multiplayer.Server;

[System.Serializable]
public class Config
{
    public int serverPort;
    public int maxPlayers;
    public string password;

    // Default config
    public Config()
    {
        serverPort = 8989;
        maxPlayers = 8;
        password = "";
    }
}
