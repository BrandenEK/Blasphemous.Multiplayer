using System;
using System.Collections.Generic;
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
            GameObject obj = new GameObject(status.name, typeof(SpriteRenderer), typeof(Animator)); // old
            obj.transform.parent = playerHolder;

            SpriteRenderer render = obj.GetComponent<SpriteRenderer>();
            //render.sortingLayerName = Core.Logic.Penitent.SpriteRenderer.sortingLayerName;
            setPlayerStatus(status, obj);
        }

        // When a player enters a scene, create a new player object
        public void addPlayer(string name)
        {
            GameObject player = new GameObject(name, typeof(SpriteRenderer), typeof(Animator));
            player.transform.parent = playerHolder;

            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            //temp
            render.sprite = Core.Logic.Penitent.SpriteRenderer.sprite;
            //render.sortingLayerName = Core.Logic.Penitent.SpriteRenderer.sortingLayerName;
        }

        // When a player leaves a scene, destroy the player object
        public void removePlayer(string name)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                UnityEngine.Object.Destroy(player);
                return;
            }

            // Error - player object doesn't exist
            Main.Multiplayer.displayNotification("Error - cant remove player");
        }

        // When receiving a player position update, find the player and change its position
        public void updatePlayerPosition(string name, Vector2 position, bool facingDirection)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                player.transform.position = new Vector3(position.x, position.y, 0);

                // Separate thing for changing direction - doesnt happen all the time
            }

            // Error - player object doesn't exist
            Main.Multiplayer.displayNotification("Error - cant update player position");
        }

        // When receiving a player position update, find the player and change its position
        public void updatePlayerAnimation(string name, string animation)
        {
            
        }

        // Finds a specified player in the scene
        private GameObject getPlayerObject(string name)
        {
            foreach (Transform child in playerHolder)
            {
                if (child.name == name)
                    return child.gameObject;
            }
            return null;
        }

        // Updates the player status whenever it receives their new data
        public void setPlayerStatus(PlayerStatus status, GameObject player = null) // old
        {
            if (player == null)
            {
                player = getPlayerObject(status.name);
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
