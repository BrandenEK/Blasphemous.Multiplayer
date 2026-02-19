using System.Text;

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

    public string Scene { get; private set; }
    public byte[] Skin { get; private set; }
    public ushort Ping { get; private set; }

    public PlayerInfo(string ip, string name, byte team)
    {
        Ip = ip;
        Name = name;
        Team = team;

        Scene = string.Empty;
        Skin = Encoding.UTF8.GetBytes("PENITENT_DEFAULT");
        Ping = 0;
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
    }

    public void UpdateSkin(byte[] skin)
    {
        Skin = skin;
    }

    public void UpdatePing(ushort ping)
    {
        Ping = ping;
    }
}
