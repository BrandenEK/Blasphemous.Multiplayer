﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlasClient.Structures
{
    public class PlayerStatus
    {
        public string currentScene; // Currently unused
        public bool specialAnimation;
        public SkinStatus skin;

        public PlayerStatus()
        {
            currentScene = "";
            skin = new SkinStatus("PENITENT_DEFAULT");
        }
    }
}
