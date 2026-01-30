using System.Linq;
using System.Text;

namespace Blasphemous.Multiplayer.Client.InputValidation;

internal class StandardValidator : ISanitizer, IValidator
{
    public string CleanServer(string text)
    {
        return CleanStringValue(text, SERVER_LENGTH, SERVER_CHARS);
    }

    public bool IsServerValid(string text)
    {
        return !string.IsNullOrEmpty(text)
            && text.Length <= SERVER_LENGTH
            && text.All(c => char.IsLetterOrDigit(c) || SERVER_CHARS.Contains(c))
            && text.Count(c => c == ':') == 1
            && ushort.TryParse(text.Substring(text.IndexOf(':') + 1), out _);
    }

    public string CleanRoom(string text)
    {
        return CleanStringValue(text, ROOM_LENGTH, ROOM_CHARS);
    }

    public bool IsRoomValid(string text)
    {
        return !string.IsNullOrEmpty(text)
            && text.Length <= ROOM_LENGTH
            && text.All(c => char.IsLetterOrDigit(c) || ROOM_CHARS.Contains(c));
    }

    public string CleanPlayer(string text)
    {
        return CleanStringValue(text, PLAYER_LENGTH, PLAYER_CHARS);
    }

    public bool IsPlayerValid(string text)
    {
        return !string.IsNullOrEmpty(text)
            && text.Length <= PLAYER_LENGTH
            && text.All(c => char.IsLetterOrDigit(c) || PLAYER_CHARS.Contains(c));
    }

    public string CleanPassword(string text)
    {
        return CleanStringValue(text, PASSWORD_LENGTH, PASSWORD_CHARS);
    }

    public bool IsPasswordValid(string text)
    {
        return string.IsNullOrEmpty(text)
            || text.Length <= PASSWORD_LENGTH
            && text.All(c => char.IsLetterOrDigit(c) || PASSWORD_CHARS.Contains(c));
    }

    public int CleanTeam(int value)
    {
        return CleanIntegerValue(value, 1, 8);
    }

    public bool IsTeamValid(int value)
    {
        return value >= 0 && value <= 8;
    }

    private string CleanStringValue(string text, int maxLength, string validChars)
    {
        var sb = new StringBuilder(text.Length);
        foreach (char c in text)
        {
            if (sb.Length < maxLength && (char.IsLetterOrDigit(c) || validChars.Contains(c)))
                sb.Append(c);
        }
        return sb.ToString();
    }

    private int CleanIntegerValue(int value, int min, int max)
    {
        return value >= min && value <= max ? value : min;
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
