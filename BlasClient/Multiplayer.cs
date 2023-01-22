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

        private bool inLevel;
        private Vector2 lastPosition;
        private string lastAnimation;

        private bool shouldSendData
        {
            get { return inLevel && client != null && client.connected; }
        }

        public void Initialize()
        {
            LevelManager.OnLevelLoaded += onLevelLoaded;
            LevelManager.OnBeforeLevelLoad += onLevelUnloaded;
            playerControl = new PlayerControl();
        }
        public void Dispose()
        {
            LevelManager.OnLevelLoaded -= onLevelLoaded;
            LevelManager.OnBeforeLevelLoad -= onLevelUnloaded;
        }

        private void onLevelLoaded(Level oldLevel, Level newLevel)
        {
            inLevel = newLevel.LevelName != "MainMenu";

            if (shouldSendData)
            {
                // Entered a new scene
                Main.UnityLog("Entering new scene: " + newLevel.LevelName);
                client.sendPlayerEnterScene(newLevel.LevelName);
            }
            playerControl.newScene();   
        }

        private void onLevelUnloaded(Level oldLevel, Level newLevel)
        {
            if (shouldSendData)
            {
                // Left a scene
                Main.UnityLog("Leaving scene: " + oldLevel.LevelName);
                client.sendPlayerLeaveScene();
            }

            inLevel = false;
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

            if (shouldSendData)
            {
                Transform penitent = Core.Logic.Penitent.transform;
                if (penitent.position.x != lastPosition.x || penitent.position.y != lastPosition.y)
                {
                    // Position has been updated
                    Main.UnityLog("Sending new player position");
                    bool dir = !Core.Logic.Penitent.SpriteRenderer.flipX;
                    client.sendPlayerPostition(penitent.position.x, penitent.position.y, dir);
                    lastPosition = new Vector2(penitent.position.x, penitent.position.y);
                }
                // Logic to check if animation clip is different

                //currentFrame++;
                //if (currentFrame > frameDelay)
                //{
                //    // Send player status
                //    Main.UnityLog("Sending player status");
                //    client.sendPlayerUpdate(getCurrentStatus());
                //    currentFrame = 0;
                //}
            }

            // temp
            if (Core.Logic.Penitent != null)
            {
                Sprite s = Core.Logic.Penitent.SpriteRenderer.sprite;
                if (!playerControl.sprites.ContainsKey(s.name))
                    playerControl.sprites.Add(s.name, s);
            }
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

        // old
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
