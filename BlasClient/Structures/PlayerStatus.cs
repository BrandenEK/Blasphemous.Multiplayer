using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlasClient.Structures
{
    public class PlayerStatus
    {
        public byte team;
        public SkinStatus skin;

        public string currentScene; // Currently unused
        public bool specialAnimation;

        public PlayerStatus()
        {
            currentScene = "";
            skin = new SkinStatus("PENITENT_DEFAULT");
        }
    }
}
