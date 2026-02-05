using Basalt.CommandParser;
using Blasphemous.Multiplayer.Common;

namespace Blasphemous.Multiplayer.Server;

public class ServerCommand : CommandData
{
    [IntegerArgument('p', "port")]
    public int Port { get; set; } = Protocol.DEFAULT_PORT;

    [IntegerArgument('m', "max-players")]
    public int MaxPlayers { get; set; } = 10;

    [StringArgument('s', "password")]
    public string Password { get; set; } = string.Empty;
}
