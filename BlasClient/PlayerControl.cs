using System.Collections.Generic;
using UnityEngine;
using Framework.Managers;

namespace BlasClient
{
    public class PlayerControl
    {
        private bool currentlyInUse;

        private Transform playerHolder;

        private string[] animNames = new string[] { "Idle", "Falling", "Run", "Jump" };
        public Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        public void loadScene(string scene)
        {
            if (scene != "MainMenu")
            {
                playerHolder = new GameObject("Player Holder").transform;
            }
            //createNewPlayer(Main.Multiplayer.getCurrentStatus());
        }

        public void unloadScene()
        {
            if (playerHolder != null)
            {
                Object.DestroyImmediate(playerHolder.gameObject);
            }
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
            render.sortingLayerName = "Player";
            Main.UnityLog("Created new player object for " + name);
        }

        // When a player leaves a scene, destroy the player object
        public void removePlayer(string name)
        {
            if (currentlyInUse)
            {
                Main.UnityLog("PlayerControl was already in use!");
                return;
            }
            currentlyInUse = true;

            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                Object.Destroy(player);
                Main.UnityLog("Removed player object for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't remove player object for " + name);
            }
            currentlyInUse = false;
        }

        // When receiving a player position update, find the player and change its position
        public void updatePlayerPosition(string name, Vector2 position, bool facingDirection)
        {
            if (currentlyInUse)
            {
                Main.UnityLog("PlayerControl was already in use!");
                return;
            }
            currentlyInUse = true;

            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                player.transform.position = new Vector3(position.x, position.y, 0);
                Main.UnityLog("Updating player object position for " + name);

                // Separate thing for changing direction - doesnt happen all the time
            }
            else
            {
                Main.UnityLog("Error: Can't find player object for " + name);
            }
            currentlyInUse = false;
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
