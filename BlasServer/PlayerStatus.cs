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
        public byte animation;
        public string skin;

        public string sceneName;

        public PlayerStatus(string name)
        {
            this.name = name;
            sceneName = "";
            skin = "";
        }

        public void updateStatus(byte[] data) // old
        {
            int startIdx = 0, length = 0;

            length = data[startIdx]; startIdx++;
            if (length != 0)
            {
                name = Encoding.UTF8.GetString(data, startIdx, length);
                startIdx += length;
            }

            xPos = BitConverter.ToSingle(data, startIdx); startIdx += 4;
            yPos = BitConverter.ToSingle(data, startIdx); startIdx += 4;
            facingDirection = BitConverter.ToBoolean(data, startIdx); startIdx += 1;

            length = data[startIdx]; startIdx++;
            if (length != 0)
            {
                //animation = Encoding.UTF8.GetString(data, startIdx, length);
                startIdx += length;
            }

            length = data[startIdx]; startIdx++;
            if (length != 0)
            {
                sceneName = Encoding.UTF8.GetString(data, startIdx, length);
                startIdx += length;
            }
        }

        public byte[] loadStatus() // old
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

            //if (animation == null)
            //{
            //    data.Add(0);
            //}
            //else
            //{
            //    //data.Add((byte)animation.Length);
            //    //data.AddRange(Encoding.UTF8.GetBytes(animation));
            //}

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

        public override string ToString()
        {
            return $"{name} is at position ({xPos},{yPos}) facing {(facingDirection ? "right" : "left")} in the animation {animation} in room {sceneName}";
        }
    }
}
