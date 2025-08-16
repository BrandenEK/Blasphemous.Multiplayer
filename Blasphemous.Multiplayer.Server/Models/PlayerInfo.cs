﻿
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

    public string sceneName;

    public PlayerInfo(string name)
    {
        this.name = name;
        team = 1;
        sceneName = "";
        skin = new byte[0];
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
