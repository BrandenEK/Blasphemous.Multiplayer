﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using Gameplay.GameControllers.Effects.Player.Recolor;

namespace BlasClient
{
    public class PlayerControl
    {
        private List<GameObject> players = new List<GameObject>();
        private List<Text> nametags = new List<Text>();

        private Transform canvas; // Optimize to not have to find these every scene change
        private GameObject textPrefab;

        // Stores the skin of each player and is updated when receiving a skin change from the server
        // A player is added to this list when first sending their skin upon connecting
        // This is only temporary until all player data is probably stored in a dict on the client
        // Once thats doen will also remove from the list when a player disconnects
        private Dictionary<string, SkinStatus> playerSkins = new Dictionary<string, SkinStatus>();

        private Dictionary<string, Vector2> queuedPositions = new Dictionary<string, Vector2>();
        private Dictionary<string, byte> queuedAnimations = new Dictionary<string, byte>();
        private Dictionary<string, bool> queuedDirections = new Dictionary<string, bool>();

        private static readonly object positionLock = new object();
        private static readonly object animationLock = new object();
        private static readonly object directionLock = new object(); // Might also need to lock players list when adding/removing

        public void loadScene(string scene)
        {
            // Remove all existing player objects and nametags
            destroyPlayers();

            // Find textPrefab
            foreach (PlayerPurgePoints obj in Object.FindObjectsOfType<PlayerPurgePoints>())
            {
                if (obj.name == "PurgePoints") { textPrefab = obj.transform.GetChild(1).gameObject; break; }
            }
            // Find canvas parent
            foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            {
                if (c.name == "Game UI") { canvas = c.transform; break; }
            }

            // Create main player's nametag
            createNameTag(Main.Multiplayer.playerName);
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

            // Check status of player skins and potentially update the textures
            foreach (string name in playerSkins.Keys)
            {
                SkinStatus player = playerSkins[name];
                if (player.updateStatus == 2)
                {
                    // Set that one update cycle has passed
                    player.updateStatus = 1;
                }
                else if (player.updateStatus == 1)
                {
                    // Set the player texture
                    setSkinTexture(name, player.skinName);
                    player.updateStatus = 0;
                }
            }

            // Update position of all name tags
            for (int i = 0; i < nametags.Count; i++)
            {
                RectTransform nametag = nametags[i].transform as RectTransform;
                string name = nametags[i].name;

                // Get player with this name
                GameObject player = name == Main.Multiplayer.playerName ? Core.Logic.Penitent.gameObject : getPlayerObject(name);
                if (player != null)
                {
                    Vector3 viewPosition = Camera.main.WorldToViewportPoint(player.transform.position + Vector3.up * 3.1f);
                    nametag.anchorMin = viewPosition;
                    nametag.anchorMax = viewPosition;
                    nametag.anchoredPosition = Vector2.zero;
                }
            }
        }

        // When disconnected from server or loading new scene, remove all players
        public void destroyPlayers()
        {
            for (int i = 0; i < players.Count; i++)
                Object.Destroy(players[i]);
            players.Clear();
            for (int i = 0; i < nametags.Count; i++)
                Object.Destroy(nametags[i].gameObject);
            nametags.Clear();
        }

        // When a player enters a scene, create a new player object
        public void addPlayer(string name)
        {
            // Create base object
            GameObject player = new GameObject("_" + name, typeof(SpriteRenderer), typeof(Animator));  // Change to create prefab at initialization, and instantiate a new instance
            players.Add(player);

            // Set up sprite rendering
            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            render.material = Core.Logic.Penitent.SpriteRenderer.material;
            render.sortingLayerName = "Player";

            // Hide player object until skin texture is set - must be delayed
            if (playerSkins.ContainsKey(name))
            {
                playerSkins[name].updateStatus = 2;
                render.enabled = false;
            }
            else
            {
                Main.UnityLog("Error: Couldn't find skin for " + name);
            }

            // Set up animations
            Animator anim = player.GetComponent<Animator>();
            anim.runtimeAnimatorController = Core.Logic.Penitent.Animator.runtimeAnimatorController;

            // Set up name tag
            createNameTag(name);

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
            Text nametag = getPlayerNametag(name);
            if (nametag != null)
            {
                nametags.Remove(nametag);
                Object.Destroy(nametag);
                Main.UnityLog("Removed nametag for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        private void updatePlayerPosition(string name, Vector2 position)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                player.transform.position = position;
                //Main.UnityLog("Updating player object position for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't update object position for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        private void updatePlayerAnimation(string name, byte animation)
        {
            GameObject player = getPlayerObject(name);
            if (player != null)
            {
                // Set a few always needed parameters
                Animator anim = player.GetComponent<Animator>();
                anim.SetBool("IS_CROUCH", false);
                //anim.SetBool("IS_DEAD") might need one for vertical attack
                // If anim is ladder climbing, set speed to 0

                // Set required parameters to keep player onject in this animation
                for (int i = 0; i < PlayerAnimations.animations[animation].parameterNames.Length; i++)
                {
                    anim.SetBool(PlayerAnimations.animations[animation].parameterNames[i], PlayerAnimations.animations[animation].parameterValues[i]);
                }
                anim.Play(PlayerAnimations.animations[animation].name);
                //Main.UnityLog("Updating player object animation for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't update object animation for " + name);
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
                //Main.UnityLog("Updating player object direction for " + name);
            }
            else
            {
                Main.UnityLog("Error: Can't update object direction for " + name);
            }
        }

        // When receiving a player skin update, change the value in the skin list
        // Should maybe be locked, but shouldn't occur frequently enough for this
        public void updatePlayerSkin(string name, string skin)
        {
            if (playerSkins.ContainsKey(name))
                playerSkins[name].skinName = skin;
            else
                playerSkins.Add(name, new SkinStatus(skin));
            Main.UnityLog("Updating player skin for " + name);
        }

        // Instantiates a nametag object
        private void createNameTag(string name)
        {
            if (canvas == null || textPrefab == null)
            {
                Main.UnityLog("Error: Failed to create nametag for " + name);
                return;
            }

            Text nametag = Object.Instantiate(textPrefab, canvas).GetComponent<Text>();
            nametag.rectTransform.sizeDelta = new Vector2(100, 50);
            nametag.rectTransform.SetAsFirstSibling();
            nametag.name = name;
            nametag.text = name;
            nametag.alignment = TextAnchor.LowerCenter;
            nametags.Add(nametag);
        }

        // Sets the skin texture of a player's object - must be delayed until after object creation
        private void setSkinTexture(string name, string skin)
        {
            // Get player object with this name
            GameObject player = getPlayerObject(name);
            if (player == null)
            {
                Main.UnityLog("Error: Can't update object skin for " + name);
                return;
            }

            // Make player visible
            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            render.enabled = true;

            // Get skin texture for this player
            Sprite palette = Core.ColorPaletteManager.GetColorPaletteById(skin);
            if (palette == null)
            {
                Main.UnityLog("Error: Couldn't find skin named " + skin);
                return;
            }

            Main.UnityLog("Setting skin texture for " + name);
            render.material.SetTexture("_PaletteTex", palette.texture);
        }

        // Finds a specified player in the scene
        private GameObject getPlayerObject(string name)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].name == "_" + name)
                    return players[i];
            }
            return null;
        }

        // Find a specified player's nametag
        private Text getPlayerNametag(string name)
        {
            for (int i = 0; i < nametags.Count; i++)
            {
                if (nametags[i].name == name)
                    return nametags[i];
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
