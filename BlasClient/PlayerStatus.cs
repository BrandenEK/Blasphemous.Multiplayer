using System;
using System.Collections.Generic;
using System.Text;

namespace BlasClient
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
            List<byte> data = new List<byte>();

            if (name == null)
            {
                data.Add(0);
            }
            else
            {
                data.Add((byte)name.Length);
                data.AddRange(Encoding.UTF8.GetBytes(name));
            }

            data.AddRange(BitConverter.GetBytes(xPos));
            data.AddRange(BitConverter.GetBytes(yPos));
            data.AddRange(BitConverter.GetBytes(facingDirection));

            if (animation == null)
            {
                data.Add(0);
            }
            else
            {
                data.Add((byte)animation.Length);
                data.AddRange(Encoding.UTF8.GetBytes(animation));
            }

            if (sceneName == null)
            {
                data.Add(0);
            }
            else
            {
                data.Add((byte)sceneName.Length);
                data.AddRange(Encoding.UTF8.GetBytes(sceneName));
            }

            return data.ToArray();
        }
    }
}
