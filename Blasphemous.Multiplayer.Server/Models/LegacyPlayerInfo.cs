using System.Text;

namespace Blasphemous.Multiplayer.Server.Models;

public class LegacyPlayerInfo
{
    public string Ip { get; }
    public string Name { get; }
    public byte Team { get; }

    public float xPos;
    public float yPos;
    public bool facingDirection;
    public byte animation;
    public byte[] skin;
    public ushort ping;

    public string sceneName;

    public LegacyPlayerInfo(string ip, string name, byte team)
    {
        Ip = ip;
        Name = name;
        Team = team;

        sceneName = string.Empty;
        skin = Encoding.UTF8.GetBytes("PENITENT_DEFAULT");
        ping = 0;
    }

    public bool isInSameScene(LegacyPlayerInfo player)
    {
        return sceneName != "" && sceneName == player.sceneName;
    }

    public override string ToString()
    {
        return $"{Name} is at position ({xPos},{yPos}) facing {(facingDirection ? "right" : "left")} in the animation {animation} in room {sceneName}";
    }
}