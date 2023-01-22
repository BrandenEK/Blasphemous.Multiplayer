using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework.Managers;

namespace BlasClient
{
    public class PlayerControl
    {
        private Transform playerHolder;

        private string[] animNames = new string[] { "Idle", "Falling", "Run", "Jump" };
        public Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        public void newScene()
        {
            playerHolder = new GameObject("Player Holder").transform;
            //createNewPlayer(Main.Multiplayer.getCurrentStatus());
        }

        // Creates a new player when either one enters the same room
        private void createNewPlayer(PlayerStatus status)
        {
            GameObject obj = new GameObject(status.name, typeof(SpriteRenderer), typeof(Animator));
            obj.transform.parent = playerHolder;

            SpriteRenderer render = obj.GetComponent<SpriteRenderer>();
            //render.sortingLayerName = Core.Logic.Penitent.SpriteRenderer.sortingLayerName;
            setPlayerStatus(status, obj);
        }

        // Updates the player status whenever it receives their new data
        public void setPlayerStatus(PlayerStatus status, GameObject player = null)
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
    }
}
