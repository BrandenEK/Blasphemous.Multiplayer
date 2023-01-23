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

        private bool inLevel;
        private Vector2 lastPosition;
        private string lastAnimation;

        private Dictionary<string, Vector2> queuedPositions = new Dictionary<string, Vector2>();
        private static readonly object playerLock = new object();

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
            playerControl.loadScene(newLevel.LevelName);   

            if (shouldSendData)
            {
                // Entered a new scene
                Main.UnityLog("Entering new scene: " + newLevel.LevelName);
                client.sendPlayerEnterScene(newLevel.LevelName);
            }
        }

        private void onLevelUnloaded(Level oldLevel, Level newLevel)
        {
            playerControl.checkHolder();
            if (shouldSendData)
            {
                // Left a scene
                Main.UnityLog("Leaving old scene");
                client.sendPlayerLeaveScene();
            }

            inLevel = false;
            playerControl.unloadScene();
        }

        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                
            }

            // Check & send updated position
            if (shouldSendData)
            {
                Transform penitent = Core.Logic.Penitent.transform;
                if (positionHasChanged(penitent.position))
                {
                    // Position has been updated
                    //Main.UnityLog("Sending new player position");
                    bool dir = !Core.Logic.Penitent.SpriteRenderer.flipX;
                    client.sendPlayerPostition(penitent.position.x, penitent.position.y, dir);
                    lastPosition = penitent.position;
                }
                // Logic to check if animation clip is different
            }

            // Update other player's data
            lock (playerLock)
            {
                foreach (string playerName in queuedPositions.Keys)
                {
                    playerControl.updatePlayerPosition(playerName, queuedPositions[playerName], true);
                }
                queuedPositions.Clear();
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

        private void queuePosition(string name, Vector2 pos)
        {
            lock (playerLock)
            {
                if (queuedPositions.ContainsKey(name))
                    queuedPositions[name] = pos;
                else
                    queuedPositions.Add(name, pos);
            }
        }

        // Received position data from server
        public void playerPositionUpdated(string playerName, float xPos, float yPos, bool facingDirection)
        {
            if (inLevel)
            {
                Main.UnityLog("Updating position of player " + playerName);
                queuePosition(playerName, new Vector2(xPos, yPos));
                //playerControl.updatePlayerPosition(playerName, new Vector2(xPos, yPos), facingDirection);
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

        public string tryConnect(string ip, string name, string password)
        {
            playerName = name;
            client = new Client(ip);
            bool result = client.Connect();

            return result ? "Successfully connected to " + ip : "Failed to connect to " + ip;
        }

        public void displayNotification(string message)
        {
            UIController.instance.ShowPopUp(message, "", 0, false);
        }
    }
}
