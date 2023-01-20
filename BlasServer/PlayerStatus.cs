using System;
using System.Collections.Generic;
using System.Text;

namespace BlasServer
{
    public class PlayerStatus
    {
        public string name;

        public float xPos;
        public float yPos;
        public bool facingDirection;
        public string animation;

        public string sceneName;

        public void updateStatus(byte[] data)
        {

        }

        public byte[] loadStatus()
        {
            return null;
        }
    }
}
