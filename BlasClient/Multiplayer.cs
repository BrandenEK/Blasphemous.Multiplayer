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
        private string playerName;

        private int frameDelay = 20;
        private int currentFrame = 0;

        private string[] animNames = new string[] { "Idle", "Falling", "Run", "Jump" };
        private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        private Transform playerHolder;

        public void Initialize()
        {
            LevelManager.OnLevelLoaded += onLevelLoaded;
        }
        public void Dispose()
        {
            LevelManager.OnLevelLoaded -= onLevelLoaded;
        }

        private void onLevelLoaded(Level oldLevel, Level newLevel)
        {
            playerHolder = new GameObject("Player Holder").transform;
            createNewPlayer(getCurrentStatus());
        }

        // Creates a new player when either one enters the same room
        private void createNewPlayer(PlayerStatus status)
        {
            GameObject obj = new GameObject(status.name, typeof(SpriteRenderer), typeof(Animator));
            obj.transform.parent = playerHolder;
            
            SpriteRenderer render = obj.GetComponent<SpriteRenderer>();
            render.sortingLayerName = Core.Logic.Penitent.SpriteRenderer.sortingLayerName;
            setPlayerStatus(status, obj);
        }
        
        // Updates the player status whenever it receives their new data
        private void setPlayerStatus(PlayerStatus status, GameObject player = null)
        {
            if (player == null)
            {
                // Must first find player
                foreach (Transform child in playerHolder)
                {
                    if (child.name == status.name)
                    {
                        player = child.gameObject;
                        break;
                    }
                }
                if (player == null)
                {
                    // Error - this player hasn't been created yet
                    return;
                }
            }

            player.transform.position = new Vector3(status.xPos, status.yPos, 0);

            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            render.flipX = !status.facingDirection;
            if (status.animation != null && sprites.ContainsKey(status.animation))
            {
                render.sprite = sprites[status.animation];
            }

            //Animator anim = obj.GetComponent<Animator>();
            //anim.speed = 0;
            //anim.runtimeAnimatorController = Core.Logic.Penitent.Animator.runtimeAnimatorController;
            //if (status.animation != null)
            //{
            //    anim.Play(status.animation, 0);
            //}
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
                    setPlayerStatus(statuses[i]);
                }
            }
        }

        public void update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                Connect();
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                createNewPlayer(getCurrentStatus());
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
            if (!sprites.ContainsKey(s.name))
                sprites.Add(s.name, s);
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
