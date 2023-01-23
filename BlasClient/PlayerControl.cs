using System.Collections.Generic;
using UnityEngine;
using Framework.Managers;

namespace BlasClient
{
    public class PlayerControl
    {
        private List<GameObject> players = new List<GameObject>();

        private Dictionary<string, Vector2> queuedPositions = new Dictionary<string, Vector2>();
        private Dictionary<string, byte> queuedAnimations = new Dictionary<string, byte>();
        private Dictionary<string, bool> queuedDirections = new Dictionary<string, bool>();

        private static readonly object positionLock = new object();
        private static readonly object animationLock = new object();
        private static readonly object directionLock = new object(); // Might also need to lock players list when adding/removing

        public void loadScene(string scene)
        {
            players.Clear();
        }

        public void unloadScene()
        {
            
        }

        // Should be optimized to not use dictionaries
        public void updatePlayers()
        {
            // Update any player's new position
            lock (positionLock)
            {
                foreach (string name in queuedPositions.Keys)
                    updatePlayerPosition(name, queuedPositions[name]);
                queuedPositions.Clear();
            }
            // Update any player's new animation
            lock (animationLock)
            {
                foreach (string name in queuedAnimations.Keys)
                    updatePlayerAnimation(name, queuedAnimations[name]);
                queuedAnimations.Clear();
            }
            // Update any player's new direction
            lock (directionLock)
            {
                foreach (string name in queuedDirections.Keys)
                    updatePlayerDirection(name, queuedDirections[name]);
                queuedDirections.Clear();
            }
        }

        // When a player enters a scene, create a new player object
        public void addPlayer(string name)
        {
            GameObject player = new GameObject(name, typeof(SpriteRenderer), typeof(Animator), typeof(PlayerAnimator));
            players.Add(player);

            // Set up sprite rendering
            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            render.sortingLayerName = "Player";
            
            // Set up animations
            Animator anim = player.GetComponent<Animator>();
            anim.runtimeAnimatorController = Core.Logic.Penitent.Animator.runtimeAnimatorController;

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
        private void updatePlayerPosition(string name, Vector2 position)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                player.transform.position = position;
                Main.UnityLog("Updating player object position for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't find player object for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        private void updatePlayerAnimation(string name, byte animation)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                Animator anim = player.GetComponent<Animator>();
                for (int i = 0; i < PlayerAnimator.animations[animation].parameterNames.Length; i++)
                {
                    anim.SetBool(PlayerAnimator.animations[animation].parameterNames[i], PlayerAnimator.animations[animation].parameterValues[i]);
                }
                anim.Play(PlayerAnimator.animations[animation].name);
                Main.UnityLog("Updating player object animation for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't find player object for " + name);
            }
        }

        // When receiving a player direction update, find the player and change its direction
        private void updatePlayerDirection(string name, bool direction)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                SpriteRenderer render = player.GetComponent<SpriteRenderer>();
                render.flipX = direction;
                Main.UnityLog("Updating player object direction for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't find player object for " + name);
            }
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

        public void queuePosition(string playerName, Vector2 position)
        {
            lock (positionLock)
            {
                if (queuedPositions.ContainsKey(playerName))
                    queuedPositions[playerName] = position;
                else
                    queuedPositions.Add(playerName, position);
            }
        }

        public void queueAnimation(string playerName, byte animation)
        {
            lock (animationLock)
            {
                if (queuedAnimations.ContainsKey(playerName))
                    queuedAnimations[playerName] = animation;
                else
                    queuedAnimations.Add(playerName, animation);
            }
        }

        public void queueDirection(string playerName, bool direction)
        {
            lock (directionLock)
            {
                if (queuedDirections.ContainsKey(playerName))
                    queuedDirections[playerName] = direction;
                else
                    queuedDirections.Add(playerName, direction);
            }
        }
    }
}
