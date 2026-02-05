
namespace Blasphemous.Multiplayer.Client.Notifications;

public class NotificationLine
{
    public string Text { get; }

    public float TimeLeft => _timeLeft;

    private float _timeLeft;

    public NotificationLine(string text, float time)
    {
        Text = text;
        _timeLeft = time;
    }

    public void SubtractTime(float time)
    {
        _timeLeft -= time;
    }
}
