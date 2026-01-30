
namespace Blasphemous.Multiplayer.Client.InputValidation;

internal interface IValidator
{
    public bool IsServerValid(string text);

    public bool IsRoomValid(string text);

    public bool IsPlayerValid(string text);

    public bool IsPasswordValid(string text);

    public bool IsTeamValid(int value);
}
