
namespace Blasphemous.Multiplayer.Client.Network;

/// <summary>
/// The data used to connect to the server
/// </summary>
public readonly struct ConnectionInfo(string serverIp, string roomName, string playerName, string password, byte teamNumber)
{
    /// <summary>
    /// The ip address of the server
    /// </summary>
    public string ServerIp { get; } = serverIp;

    /// <summary>
    /// The name of the room
    /// </summary>
    public string RoomName { get; } = roomName;

    /// <summary>
    /// The name of the player
    /// </summary>
    public string PlayerName { get; } = playerName;

    /// <summary>
    /// The optional password
    /// </summary>
    public string Password { get; } = password;

    /// <summary>
    /// The team number (1-8)
    /// </summary>
    public byte TeamNumber { get; } = teamNumber;
}
