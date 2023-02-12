﻿
namespace BlasServer
{
    public class PlayerStatus
    {
        public string name;

        public float xPos;
        public float yPos;
        public bool facingDirection;
        public byte animation;
        public string skin;

        public string sceneName;

        public PlayerStatus(string name)
        {
            this.name = name;
            sceneName = "";
            skin = "";
        }

        public bool isInSameScene(PlayerStatus player)
        {
            return sceneName != "" && sceneName == player.sceneName;
        }

        public override string ToString()
        {
            return $"{name} is at position ({xPos},{yPos}) facing {(facingDirection ? "right" : "left")} in the animation {animation} in room {sceneName}";
        }
    }
}
