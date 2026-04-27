using Basalt.CommandParser;
using Basalt.CommandParser.Attributes;
using Blasphemous.Multiplayer.Common;

namespace Blasphemous.Multiplayer.Server;

public class ServerArguments : ProgramArguments
{
    [IntegerArgument("port", "p", "Which port the server will listen on (1-65535)")]
    public int Port { get; set; } = Protocol.DEFAULT_PORT;

    [IntegerArgument("max-players", "mp", "The maximum number of players allowed in the server")]
    public int MaxPlayers { get; set; } = 10;

    [StringArgument("password", "ps", "The password to use")]
    public string Password { get; set; } = string.Empty;
}
