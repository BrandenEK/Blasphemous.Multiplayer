
namespace Blasphemous.Multiplayer.Server.Models;

public class PlayerInfo
{
    public string name;
    public byte team;

    public float xPos;
    public float yPos;
    public bool facingDirection;
    public byte animation;
    public byte[] skin;
    public ushort ping;

    public string sceneName;

    public PlayerInfo(string name, byte team)
    {
        this.name = name;
        this.team = team;
        sceneName = "";
        skin = new byte[0];
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
