using System.Collections.Generic;
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

        private Transform canvas;
        private GameObject textPrefab;

        // Stores the skin of each player and is updated when receiving a skin change from the server
        // A player is added to this list when first sending their skin upon connecting
        // This is only temporary until all player data is probably stored in a dict on the client
        // Once thats doen will also remove from the list when a player disconnects
        private Dictionary<string, string> playerSkins = new Dictionary<string, string>();

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
            GameObject player = new GameObject(name, typeof(SpriteRenderer), typeof(Animator), typeof(PlayerAnimator), typeof(ColorPaletteSwapper));  // Change to create prefab at initialization, and instantiate a new instance
            players.Add(player);

            // Set up sprite rendering
            SpriteRenderer render = player.GetComponent<SpriteRenderer>();
            render.material = Core.Logic.Penitent.SpriteRenderer.material;
            render.sortingLayerName = "Player";

            // Set up animations
            Animator anim = player.GetComponent<Animator>();
            anim.runtimeAnimatorController = Core.Logic.Penitent.Animator.runtimeAnimatorController;

            // Set up skin
            setSkinTexture(name, render);

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

        // When receiving a player skin update, change the value in the skin list
        // Should maybe be locked, but shouldn't occur frequently enough for this
        public void updatePlayerSkin(string name, string skin)
        {
            if (playerSkins.ContainsKey(name))
                playerSkins[name] = skin;
            else
                playerSkins.Add(name, skin);
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

        // Sets the skin texture of a player's object
        private void setSkinTexture(string name, SpriteRenderer render)
        {
            if (!playerSkins.TryGetValue(name, out string skin))
            {
                Main.UnityLog("Error: Couldn't find skin for " + name);
                skin = "PENITENT_DEFAULT";
            }

            Sprite palette = Core.ColorPaletteManager.GetColorPaletteById(skin);
            if (palette != null)
            {
                render.material.SetTexture("_PaletteTex", palette.texture);
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
