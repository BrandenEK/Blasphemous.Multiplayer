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

        private int frameDelay = 120;
        private int currentFrame = 0;

        private string[] animNames = new string[] { "Idle", "Falling", "Run", "Jump" };
        private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

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

            createNewPlayer();
        }

        private void createNewPlayer()
        {
            // Create new player based on test playerStatus
            PlayerStatus status = getCurrentStatus();

            GameObject obj = new GameObject("Test player", typeof(SpriteRenderer), typeof(Animator));
            obj.transform.position = new Vector3(status.xPos, status.yPos, 0);

            SpriteRenderer render = obj.GetComponent<SpriteRenderer>();
            render.flipX = !status.facingDirection;
            render.sortingLayerName = Core.Logic.Penitent.SpriteRenderer.sortingLayerName;

            //if (status.animation != null && sprites.ContainsKey(status.animation))
            //{
            //    render.sprite = sprites[status.animation];
            //}

            Animator anim = obj.GetComponent<Animator>();
            anim.speed = 0;
            anim.runtimeAnimatorController = Core.Logic.Penitent.Animator.runtimeAnimatorController;
            if (status.animation != null)
            {
                anim.Play(status.animation, 0);
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
                createNewPlayer();
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
            //Sprite s = Core.Logic.Penitent.SpriteRenderer.sprite;
            //if (!sprites.ContainsKey(s.name))
            //    sprites.Add(s.name, s);
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

                //status.animation = penitent.SpriteRenderer.sprite.name;

                Animator anim = penitent.Animator;
                for (int i = 0; i < animNames.Length; i++)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName(animNames[i]))
                    {
                        status.animation = animNames[i];
                        break;
                    }
                }
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
