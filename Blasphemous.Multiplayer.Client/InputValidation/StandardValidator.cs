using System.Linq;

namespace Blasphemous.Multiplayer.Client.InputValidation;

internal class StandardValidator : IValidator
{
    public bool IsServerValid(string text)
    {
        return !string.IsNullOrEmpty(text)
            && text.Length <= SERVER_LENGTH
            && text.All(c => char.IsLetterOrDigit(c) || SERVER_CHARS.Contains(c))
            && text.Count(c => c == ':') == 1
            && ushort.TryParse(text.Substring(text.IndexOf(':') + 1), out _);
    }

    public bool IsRoomValid(string text)
    {
        return !string.IsNullOrEmpty(text)
            && text.Length <= ROOM_LENGTH
            && text.All(c => char.IsLetterOrDigit(c) || ROOM_CHARS.Contains(c));
    }

    public bool IsPlayerValid(string text)
    {
        return !string.IsNullOrEmpty(text)
            && text.Length <= PLAYER_LENGTH
            && text.All(c => char.IsLetterOrDigit(c) || PLAYER_CHARS.Contains(c));
    }

    public bool IsPasswordValid(string text)
    {
        return string.IsNullOrEmpty(text)
            || text.Length <= PASSWORD_LENGTH
            && text.All(c => char.IsLetterOrDigit(c) || PASSWORD_CHARS.Contains(c));
    }

    public bool IsTeamValid(int value)
    {
        return value >= 0 && value <= 8;
    }

    private const int SERVER_LENGTH = 64;
    private const int ROOM_LENGTH = 16;
    private const int PLAYER_LENGTH = 16;
    private const int PASSWORD_LENGTH = 32;

    private const string SERVER_CHARS = "-:.";
    private const string ROOM_CHARS = "_-";
    private const string PLAYER_CHARS = "_-.' ";
    private const string PASSWORD_CHARS = "_-.";
}
