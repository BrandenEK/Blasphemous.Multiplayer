using System.Text;

namespace Blasphemous.Multiplayer.Server.Models;

public class PlayerInfo
{
    public string Ip { get; }

    public string name;
    public byte team;

    public float xPos;
    public float yPos;
    public bool facingDirection;
    public byte animation;
    public byte[] skin;
    public ushort ping;

    public string sceneName;

    public PlayerInfo(string ip, string name, byte team)
    {
        Ip = ip;
        this.name = name;
        this.team = team;
        sceneName = string.Empty;
        skin = Encoding.UTF8.GetBytes("PENITENT_DEFAULT");
        ping = 0;
    }

    public bool isInSameScene(PlayerInfo player)
    {
        return sceneName != "" && sceneName == player.sceneName;
    }

    public override string ToString()
    {
        return $"{name} is at position ({xPos},{yPos}) facing {(facingDirection ? "right" : "left")} in the animation {animation} in room {sceneName}";
    }
}
