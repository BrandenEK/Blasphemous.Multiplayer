
namespace Blasphemous.Multiplayer.Server.Models;

public class PlayerInfo
{
    public string Ip { get; }
    public string Name { get; }
    public byte Team { get; }

    public float XPosition { get; private set; } = 0;
    public float YPosition { get; private set; } = 0;
    public byte Animation { get; private set; } = 0;
    public bool Direction { get; private set; } = true;

    public string Scene { get; private set; } = string.Empty;
    public string Skin { get; private set; } = "PENITENT_DEFAULT";
    public ushort Ping { get; private set; } = 0;

    public PlayerInfo(string ip, string name, byte team)
    {
        Ip = ip;
        Name = name;
        Team = team;
    }

    public void UpdatePosition(float x, float y)
    {
        XPosition = x;
        YPosition = y;
    }

    public void UpdateAnimation(byte anim)
    {
        Animation = anim;
    }

    public void UpdateDirection(bool dir)
    {
        Direction = dir;
    }

    public void UpdateScene(string scene)
    {
        Scene = scene;

        if (!string.IsNullOrEmpty(scene))
            return;

        XPosition = 0;
        YPosition = 0;
        Animation = 0;
        Direction = false;
    }

    public void UpdateSkin(string skin)
    {
        Skin = skin;
    }

    public void UpdatePing(ushort ping)
    {
        Ping = ping;
    }
}
