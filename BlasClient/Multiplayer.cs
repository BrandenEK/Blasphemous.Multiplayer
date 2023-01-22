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
            playerControl.newScene();   

            if (shouldSendData)
            {
                // Entered a new scene
                Main.UnityLog("Entering new scene: " + newLevel.LevelName);
                client.sendPlayerEnterScene(newLevel.LevelName);
            }
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
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {

            }

            if (shouldSendData)
            {
                Transform penitent = Core.Logic.Penitent.transform;
                if (positionHasChanged(penitent.position))
                {
                    // Position has been updated
                    Main.UnityLog("Sending new player position");
                    bool dir = !Core.Logic.Penitent.SpriteRenderer.flipX;
                    client.sendPlayerPostition(penitent.position.x, penitent.position.y, dir);
                    lastPosition = penitent.position;
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

        private bool positionHasChanged(Vector2 currentPosition)
        {
            float cutoff = 0.03f;
            return Mathf.Abs(currentPosition.x - lastPosition.x) > cutoff || Mathf.Abs(currentPosition.y - lastPosition.y) > cutoff;
        }

        // Received position data from server
        public void playerPositionUpdated(string playerName, float xPos, float yPos, bool facingDirection)
        {
            if (inLevel)
            {
                Main.UnityLog("Updating position of player " + playerName);
                playerControl.updatePlayerPosition(playerName, new Vector2(xPos, yPos), facingDirection);
            }
            else
            {
                Main.UnityLog("Won't receive position of player " + playerName);
            }
        }

        // Received animation data from server
        public void playerAnimationUpdated(string playerName, string animation)
        {
            if (inLevel)
            {
                Main.UnityLog("Updating animation of player " + playerName);
                playerControl.updatePlayerAnimation(playerName, animation);
            }
            else
            {
                Main.UnityLog("Won't receive animation of player " + playerName);
            }
        }

        // Received enterScene data from server
        public void playerEnteredScene(string playerName)
        {
            if (inLevel)
            {
                Main.UnityLog("Adding player " + playerName + " to scene");
                playerControl.addPlayer(playerName);
            }
            else
            {
                Main.UnityLog("Won't receive new scene of player " + playerName);
            }
        }

        // Received leftScene data from server
        public void playerLeftScene(string playerName)
        {
            if (inLevel)
            {
                Main.UnityLog("Removing player " + playerName+ " from scene");
                playerControl.removePlayer(playerName);
            }
            else
            {
                Main.UnityLog("Won't receive old scene of player " + playerName);
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

        public string tryConnect(string ip, string name, string password)
        {
            playerName = name;
            client = new Client(ip);
            bool result = client.Connect();

            return result ? "Successfully connected to " + ip : "Failed to connect to " + ip;
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
