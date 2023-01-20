using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gameplay.UI;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;

namespace BlasClient
{
    public class Multiplayer
    {
        private Client client;
        private string playerName;

        private int frameDelay = 120;
        private int currentFrame = 0;

        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                Connect();
            }

            if (client.connected)
            {
                currentFrame++;
                if (currentFrame > frameDelay)
                {
                    // Send player status
                    Main.UnityLog("Sending player status");
                    client.sendPlayerUpdate(getCurrentStatus());
                    currentFrame = 0;
                }
            }
        }

        private PlayerStatus getCurrentStatus()
        {
            PlayerStatus status = new PlayerStatus();
            status.name = playerName;

            Penitent penitent = Core.Logic.Penitent;
            if (penitent != null)
            {
                status.xPos = penitent.transform.position.x;
                status.yPos = penitent.transform.position.y;
                status.facingDirection = penitent.GetOrientation() == Framework.FrameworkCore.EntityOrientation.Right ? true : false;
                status.animation = "test";
            }
            if (Core.LevelManager.currentLevel != null && Core.LevelManager.currentLevel.LevelName != "MainMenu")
            {
                status.sceneName = Core.LevelManager.currentLevel.LevelName;
            }
            return status;
        }

        public void Connect()
        {
            playerName = "Test";
            client = new Client("localhost");
            client.Connect();
        }

        public void displayNotification(string message)
        {
            UIController.instance.ShowPopUp(message, "", 0, false);
        }
    }
}
