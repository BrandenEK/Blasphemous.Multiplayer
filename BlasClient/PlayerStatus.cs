﻿using System;
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

        // Returns the length of the playerstatus in data
        public int updateStatus(byte[] data, int startIdx)
        {
            int length = data[startIdx]; startIdx++;
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
                animation = Encoding.UTF8.GetString(data, startIdx, length);
                startIdx += length;
            }

            length = data[startIdx]; startIdx++;
            if (length != 0)
            {
                sceneName = Encoding.UTF8.GetString(data, startIdx, length);
                startIdx += length;
            }

            return startIdx;
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

        public override string ToString()
        {
            return $"{name} is at position ({xPos},{yPos}) facing {(facingDirection ? "right" : "left")} in the animation {animation} in room {sceneName}";
        }
    }
}
