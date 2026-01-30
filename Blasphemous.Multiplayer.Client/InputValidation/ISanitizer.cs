
namespace Blasphemous.Multiplayer.Client.InputValidation;

internal interface ISanitizer
{
    public string CleanServer(string text);

    public string CleanRoom(string text);

    public string CleanPlayer(string text);

    public string CleanPassword(string text);

    public int CleanTeam(int value);
}
