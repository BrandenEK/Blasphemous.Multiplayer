
namespace Blasphemous.Multiplayer.Common.Enums;

public enum RefusalType
{
    Accepted = 0,
    // Network
    Connection = 10,
    Protocol = 11,
    DuplicateIp = 12,
    // Parameters
    DuplicateName = 20,
    Password = 21,
    // Server
    PlayerLimit = 30,
    Banned = 31,
}
