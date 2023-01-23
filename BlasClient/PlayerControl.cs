using System.Collections.Generic;
using UnityEngine;
using Framework.Managers;

namespace BlasClient
{
    public class PlayerControl
    {
        private List<GameObject> players = new List<GameObject>();

        private string[] animNames = new string[] { "Idle", "Falling", "Run", "Jump" };
        public Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        public void loadScene(string scene)
        {
            players.Clear();
        }

        public void unloadScene()
        {
            
        }

        // When a player enters a scene, create a new player object
        public void addPlayer(string name)
        {
            GameObject player = new GameObject(name, typeof(SpriteRenderer), typeof(Animator));
            players.Add(player);

            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            //temp
            render.sprite = Core.Logic.Penitent.SpriteRenderer.sprite;
            render.sortingLayerName = "Player";
            Main.UnityLog("Created new player object for " + name);
        }

        // When a player leaves a scene, destroy the player object
        public void removePlayer(string name)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                players.Remove(player);
                Object.Destroy(player);
                Main.UnityLog("Removed player object for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't remove player object for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        public void updatePlayerPosition(string name, Vector2 position, bool facingDirection)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                player.transform.position = position;
                Main.UnityLog("Updating player object position for " + name);

                // Separate thing for changing direction - doesnt happen all the time
            }
            else
            {
                Main.UnityLog("Error: Can't find player object for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        public void updatePlayerAnimation(string name, string animation)
        {
            
        }

        // Finds a specified player in the scene
        private GameObject getPlayerObject(string name)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == name)
                    return players[i];
            }
            return null;
        }
    }
}
