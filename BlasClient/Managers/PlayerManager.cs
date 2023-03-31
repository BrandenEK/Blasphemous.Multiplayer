using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using Tools.Level;
using Tools.Level.Interactables;
using BlasClient.MonoBehaviours;

namespace BlasClient.Managers
{
    public class PlayerManager
    {
        private List<OtherPenitent> players = new List<OtherPenitent>();
        private List<Text> nametags = new List<Text>();

        // Queued player updates
        private Dictionary<string, bool> queuedPlayers = new Dictionary<string, bool>();
        private Dictionary<string, Vector2> queuedPositions = new Dictionary<string, Vector2>();
        private Dictionary<string, byte> queuedAnimations = new Dictionary<string, byte>();
        private Dictionary<string, bool> queuedDirections = new Dictionary<string, bool>();

        private static readonly object playerLock = new object();
        private static readonly object positionLock = new object();
        private static readonly object animationLock = new object();
        private static readonly object directionLock = new object();

        public void loadScene(string scene)
        {
            // Remove all existing player objects and nametags
            destroyPlayers();

            // Create any players that are already in this scene
            foreach (string playerName in Main.Multiplayer.playerList.getAllPlayers())
            {
                if (Main.Multiplayer.playerList.getPlayerScene(playerName) == scene)
                    addPlayer(playerName);
            }

            // Load stored objects
            Transform tempCanvas = storedCanvas;
            GameObject tempText = storedTextPrefab;
            RuntimeAnimatorController tempAnimator = storedPenitentAnimator;
            Material tempMaterial = storedPenitentMaterial;

            // Add special animation checker to certain interactors
            int count = 0;
            foreach (Interactable interactable in Object.FindObjectsOfType<Interactable>())
            {
                System.Type type = interactable.GetType();
                if (type != typeof(PrieDieu) && type != typeof(CollectibleItem) && type != typeof(Chest) && type != typeof(Lever) && type != typeof(Door))
                    continue;

                foreach (Transform child in interactable.transform)
                {
                    if (child.name.ToLower().Contains("interactor"))
                    {
                        // Only add this to the interactor animator of certain interactables
                        child.gameObject.AddComponent<SpecialAnimationChecker>();
                        count++;
                        break;
                    }
                }
            }
            if (scene == "D17Z01S01")
            {
                // Add this to the fake penitent intro animator
                GameObject fakePenitent = GameObject.Find("FakePenitent");
                if (fakePenitent != null) fakePenitent.AddComponent<SpecialAnimationChecker>();
                count++;
            }
            Main.Multiplayer.Log("Adding special animation checkers to " + count + " objects!");

            // Create main player's nametag
            if (Main.Multiplayer.connectedToServer)
                createPlayerNameTag();
        }

        public void unloadScene()
        {
            
        }

        // Should be optimized to not use dictionaries
        public void updatePlayers()
        {
            // Add or remove any new player objects
            lock (playerLock)
            {
                if (queuedPlayers.Count > 0)
                {
                    foreach (string name in queuedPlayers.Keys)
                    {
                        if (queuedPlayers[name])
                            addPlayer(name);
                        else
                            removePlayer(name);
                    }
                    queuedPlayers.Clear();
                }
            }
            // Update any player's new position
            lock (positionLock)
            {
                if (queuedPositions.Count > 0)
                {
                    foreach (string name in queuedPositions.Keys)
                        updatePlayerPosition(name, queuedPositions[name]);
                    queuedPositions.Clear();
                }
            }
            // Update any player's new animation
            lock (animationLock)
            {
                if (queuedAnimations.Count > 0)
                {
                    foreach (string name in queuedAnimations.Keys)
                        updatePlayerAnimation(name, queuedAnimations[name]);
                    queuedAnimations.Clear();
                }
            }
            // Update any player's new direction
            lock (directionLock)
            {
                if (queuedDirections.Count > 0)
                {
                    foreach (string name in queuedDirections.Keys)
                        updatePlayerDirection(name, queuedDirections[name]);
                    queuedDirections.Clear();
                }
            }

            // Check status of player skins and potentially update the textures
            foreach (string playerName in Main.Multiplayer.playerList.getAllPlayers())
            {
                byte currentSkinStatus = Main.Multiplayer.playerList.getPlayerSkinUpdateStatus(playerName);
                if (currentSkinStatus == 2)
                {
                    // Set that one update cycle has passed
                    Main.Multiplayer.playerList.setPlayerSkinUpdateStatus(playerName, 1);
                }
                else if (currentSkinStatus == 1)
                {
                    // Set the player texture
                    setSkinTexture(playerName, Main.Multiplayer.playerList.getPlayerSkinTexture(playerName));
                    Main.Multiplayer.playerList.setPlayerSkinUpdateStatus(playerName, 0);
                }
            }

            // Update position of all name tags
            for (int i = 0; i < nametags.Count; i++)
            {
                RectTransform nametag = nametags[i].transform as RectTransform;
                string name = nametags[i].name;

                // Get player with this name
                GameObject player = null;
                if (name == Main.Multiplayer.playerName)
                {
                    player = Core.Logic.Penitent.gameObject;
                }
                else
                {
                    OtherPenitent penitent = getPlayerObject(name);
                    if (penitent != null) player = penitent.gameObject;
                }
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
            {
                if (players[i] != null)
                    Object.Destroy(players[i].gameObject);
            }
            players.Clear();
            for (int i = 0; i < nametags.Count; i++)
            {
                if (nametags[i] != null)
                    Object.Destroy(nametags[i].gameObject);
            }
            nametags.Clear();
        }

        // When a player enters a scene, create a new player object
        private void addPlayer(string name)
        {
            // Create & setup new penitent object
            GameObject playerObject = new GameObject("_" + name);
            OtherPenitent penitent = playerObject.AddComponent<OtherPenitent>();
            penitent.createPenitent(name, storedPenitentAnimator, storedPenitentMaterial);
            players.Add(penitent);

            // If in beginning room, add fake penitent controller
            if (Core.LevelManager.currentLevel.LevelName == "D17Z01S01")
                playerObject.AddComponent<FakePenitentIntro>();

            // Set up name tag
            if (Main.Multiplayer.config.displayNametags)
                createNameTag(name, Main.Multiplayer.playerList.getPlayerTeam(name) == Main.Multiplayer.playerTeam);

            Main.Multiplayer.Log("Created new player object for " + name);
        }

        // When a player leaves a scene, destroy the player object
        private void removePlayer(string name)
        {
            OtherPenitent penitent = getPlayerObject(name);
            if (penitent != null)
            {
                players.Remove(penitent);
                Object.Destroy(penitent.gameObject);
                Main.Multiplayer.Log("Removed player object for " + name);
            }
            else
            {
                Main.Multiplayer.LogWarning("Error: Can't remove player object for " + name);
            }
            Text nametag = getPlayerNametag(name);
            if (nametag != null)
            {
                nametags.Remove(nametag);
                Object.Destroy(nametag);
                Main.Multiplayer.Log("Removed nametag for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        private void updatePlayerPosition(string name, Vector2 position)
        {
            OtherPenitent penitent = getPlayerObject(name);
            if (penitent != null)
            {
                penitent.updatePosition(position);
                //Main.Multiplayer.Log("Updating player object position for " + name);
            }
            else
            {
                Main.Multiplayer.LogWarning("Error: Can't update object position for " + name);
            }
        }

        // When receiving a player position update, find the player and change its position
        private void updatePlayerAnimation(string name, byte animation)
        {
            OtherPenitent penitent = getPlayerObject(name);
            if (penitent != null)
            {
                penitent.updateAnimation(animation);
                //Main.Multiplayer.Log("Updating player object animation for " + name);
            }
            else
            {
                Main.Multiplayer.LogWarning("Error: Can't update object animation for " + name);
            }
        }

        // When receiving a player direction update, find the player and change its direction
        private void updatePlayerDirection(string name, bool direction)
        {
            OtherPenitent penitent = getPlayerObject(name);
            if (penitent != null)
            {
                penitent.updateDirection(direction);
                //Main.Multiplayer.Log("Updating player object direction for " + name);
            }
            else
            {
                Main.Multiplayer.LogWarning("Error: Can't update object direction for " + name);
            }
        }

        // Instantiates a nametag object
        private void createNameTag(string name, bool friendlyTeam)
        {
            Transform parent = storedCanvas; GameObject text = storedTextPrefab;

            if (parent == null || text == null)
            {
                Main.Multiplayer.LogWarning("Error: Failed to create nametag for " + name);
                return;
            }

            Text nametag = Object.Instantiate(text, parent).GetComponent<Text>();
            nametag.rectTransform.sizeDelta = new Vector2(100, 50);
            nametag.rectTransform.SetAsFirstSibling();
            nametag.name = name;
            nametag.text = name;
            nametag.alignment = TextAnchor.LowerCenter;
            nametag.color = friendlyTeam ? new Color(0.671f, 0.604f, 0.247f) : Color.red;
            nametags.Add(nametag);
        }

        // Creates a nametag specifically for the main player
        public void createPlayerNameTag()
        {
            if (Main.Multiplayer.config.displayNametags && Main.Multiplayer.config.displayOwnNametag)
                createNameTag(Main.Multiplayer.playerName, true);
        }

        // Updates the colors of all nametags in the scene when someone changes teams
        public void refreshNametagColors()
        {
            for (int i = 0; i < nametags.Count; i++)
            {
                bool friendlyTeam = nametags[i].name == Main.Multiplayer.playerName || Main.Multiplayer.playerTeam == Main.Multiplayer.playerList.getPlayerTeam(nametags[i].name);
                nametags[i].GetComponent<Text>().color = friendlyTeam ? new Color(0.671f, 0.604f, 0.247f) : Color.red;
            }
        }

        // Sets the skin texture of a player's object - must be delayed until after object creation
        private void setSkinTexture(string name, Texture2D skinTexture)
        {
            // Get player object with this name
            OtherPenitent penitent = getPlayerObject(name);
            if (penitent == null)
            {
                Main.Multiplayer.LogWarning("Error: Can't update object skin for " + name);
                return;
            }

            Main.Multiplayer.Log("Setting skin texture for " + name);
            penitent.updateSkin(skinTexture);
        }

        // Finds a specified player in the scene
        public OtherPenitent getPlayerObject(string name)
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

        public void queuePlayer(string playerName, bool addition)
        {
            lock (playerLock)
            {
                queuedPlayers.Add(playerName, addition);
            }
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

        private Transform m_canvas;
        private Transform storedCanvas
        {
            get
            {
                if (m_canvas == null)
                {
                    Main.Multiplayer.LogWarning("Canvas was null - Loading now");
                    foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
                    {
                        if (c.name == "Game UI") { m_canvas = c.transform; break; }
                    }
                }
                return m_canvas;
            }
        }

        private GameObject m_textPrefab;
        private GameObject storedTextPrefab
        {
            get
            {
                if (m_textPrefab == null)
                {
                    Main.Multiplayer.LogWarning("Text prefab was null - Loading now");
                    foreach (PlayerPurgePoints obj in Object.FindObjectsOfType<PlayerPurgePoints>())
                    {
                        if (obj.name == "PurgePoints") { m_textPrefab = obj.transform.GetChild(1).gameObject; break; }
                    }
                }
                return m_textPrefab;
            }
        }

        private RuntimeAnimatorController m_penitentAnimator;
        private RuntimeAnimatorController storedPenitentAnimator
        {
            get
            {
                if (m_penitentAnimator == null)
                {
                    Main.Multiplayer.LogWarning("Penitent animator controller was null - Loading now");
                    if (Core.Logic.Penitent != null)
                        m_penitentAnimator = Core.Logic.Penitent.Animator.runtimeAnimatorController;
                }
                return m_penitentAnimator;
            }
        }

        private Material m_penitentMaterial;
        private Material storedPenitentMaterial
        {
            get
            {
                if (m_penitentMaterial == null)
                {
                    Main.Multiplayer.LogWarning("Penitent material was null - Loading now");
                    if (Core.Logic.Penitent != null)
                        m_penitentMaterial = Core.Logic.Penitent.SpriteRenderer.material;
                }
                return m_penitentMaterial;
            }
        }
    }
}
