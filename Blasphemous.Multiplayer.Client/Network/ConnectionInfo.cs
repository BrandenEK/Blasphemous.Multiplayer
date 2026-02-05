using Blasphemous.Multiplayer.Common;
using Newtonsoft.Json;

namespace Blasphemous.Multiplayer.Client.Network;

/// <summary>
/// The data used to connect to the server
/// </summary>
public readonly struct ConnectionInfo
{
    /// <summary>
    /// Creates a new connection info
    /// </summary>
    [JsonConstructor]
    public ConnectionInfo(string serverIp, string roomName, string playerName, string password, byte teamNumber)
    {
        ServerIp = serverIp ?? string.Empty;
        RoomName = roomName ?? string.Empty;
        PlayerName = playerName ?? string.Empty;
        Password = password ?? string.Empty;
        TeamNumber = teamNumber;
    }

    /// <summary>
    /// Creates a new connection info with default parameters
    /// </summary>
    public ConnectionInfo()
    {
        ServerIp = $"{Protocol.DEFAULT_IP}:{Protocol.DEFAULT_PORT}";
        RoomName = string.Empty;
        PlayerName = string.Empty;
        Password = string.Empty;
        TeamNumber = 1;
    }

    /// <summary>
    /// The ip address of the server
    /// </summary>
    public string ServerIp { get; }

    /// <summary>
    /// The name of the room
    /// </summary>
    public string RoomName { get; }

    /// <summary>
    /// The name of the player
    /// </summary>
    public string PlayerName { get; }

    /// <summary>
    /// The optional password
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// The team number (1-8)
    /// </summary>
    public byte TeamNumber { get; }
}
