
namespace Blasphemous.Multiplayer.Client.InputValidation;

internal class StandardValidator : IValidator
{
    public bool IsServerValid(string text)
    {
        return true;
    }

    public bool IsRoomValid(string text)
    {
        return text == "debug";
    }

    public bool IsPlayerValid(string text)
    {
        return false;
    }

    public bool IsPasswordValid(string text)
    {
        return true;
    }

    public bool IsTeamValid(int value)
    {
        return true;
    }
}
