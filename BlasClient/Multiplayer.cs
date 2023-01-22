using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gameplay.UI;
using Framework.Managers;
using Framework.FrameworkCore;
using Gameplay.GameControllers.Penitent;

namespace BlasClient
{
    public class Multiplayer
    {
        private Client client;
        private PlayerControl playerControl;
        private string playerName;

        private int frameDelay = 20;
        private int currentFrame = 0;

        public void Initialize()
        {
            LevelManager.OnLevelLoaded += onLevelLoaded;
            playerControl = new PlayerControl();
        }
        public void Dispose()
        {
            LevelManager.OnLevelLoaded -= onLevelLoaded;
        }

        private void onLevelLoaded(Level oldLevel, Level newLevel)
        {
            playerControl.newScene();   
        }

        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                Connect();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {

            }

            if (client != null && client.connected)
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

            // temp
            Sprite s = Core.Logic.Penitent.SpriteRenderer.sprite;
            if (!playerControl.sprites.ContainsKey(s.name))
                playerControl.sprites.Add(s.name, s);
        }

        // Temp
        public PlayerStatus getCurrentStatus()
        {
            PlayerStatus status = new PlayerStatus();
            status.name = playerName;

            Penitent penitent = Core.Logic.Penitent;
            if (penitent != null)
            {
                status.xPos = penitent.transform.position.x;
                status.yPos = penitent.transform.position.y;
                status.facingDirection = penitent.GetOrientation() == EntityOrientation.Right ? true : false;

                status.animation = penitent.SpriteRenderer.sprite.name;

                //Animator anim = penitent.Animator;
                //for (int i = 0; i < animNames.Length; i++)
                //{
                //    if (anim.GetCurrentAnimatorStateInfo(0).IsName(animNames[i]))
                //    {
                //        status.animation = animNames[i];
                //        break;
                //    }
                //}
            }
            if (Core.LevelManager.currentLevel != null && Core.LevelManager.currentLevel.LevelName != "MainMenu")
            {
                status.sceneName = Core.LevelManager.currentLevel.LevelName;
            }
            return status;
        }

        public void updatePlayers(List<PlayerStatus> statuses)
        {
            // Skip updating players if not loaded into a real level
            if (Core.LevelManager.InsideChangeLevel || Core.LevelManager.currentLevel == null || Core.LevelManager.currentLevel.LevelName == "MainMenu")
                return;

            string currentScene = Core.LevelManager.currentLevel.LevelName;
            for (int i = 0; i < statuses.Count; i++)
            {
                if (statuses[i].sceneName == currentScene)
                {
                    // Move this check to inside the server to only send new player data if in same scene
                    playerControl.setPlayerStatus(statuses[i]);
                }
            }
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
