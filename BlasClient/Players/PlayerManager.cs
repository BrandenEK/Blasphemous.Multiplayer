using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using Tools.Level;
using Tools.Level.Interactables;
using BlasClient.MonoBehaviours;

namespace BlasClient.Players
{
    public class PlayerManager
    {
        private List<Text> nametags = new List<Text>();

        // Queued player updates
        private Dictionary<string, bool> queuedPlayers = new Dictionary<string, bool>();
        private Dictionary<string, Vector2> queuedPositions = new Dictionary<string, Vector2>();
        private Dictionary<string, byte> queuedAnimations = new Dictionary<string, byte>();
        private Dictionary<string, bool> queuedDirections = new Dictionary<string, bool>();


        public void LevelLoaded(string scene)
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
            RuntimeAnimatorController tempPenitentAnimator = storedPenitentAnimator;
            RuntimeAnimatorController tempSwordAnimator = storedSwordAnimator;
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
            if (Main.Multiplayer.NetworkManager.IsConnected)
                createPlayerNameTag();
        }

        // Should be optimized to not use dictionaries
        public void Update()
        {
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
                if (name == Main.Multiplayer.PlayerName)
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
                createNameTag(Main.Multiplayer.PlayerName, true);
        }

        // Updates the colors of all nametags in the scene when someone changes teams
        public void refreshNametagColors()
        {
            for (int i = 0; i < nametags.Count; i++)
            {
                bool friendlyTeam = nametags[i].name == Main.Multiplayer.PlayerName || Main.Multiplayer.PlayerTeam == Main.Multiplayer.playerList.getPlayerTeam(nametags[i].name);
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

        private RuntimeAnimatorController m_SwordAnimator;
        private RuntimeAnimatorController storedSwordAnimator
        {
            get
            {
                if (m_SwordAnimator == null)
                {
                    Main.Multiplayer.LogWarning("Sword animator controller was null - Loading now");
                    if (Core.Logic.Penitent != null)
                        m_SwordAnimator = Core.Logic.Penitent.GetComponentInChildren<Gameplay.GameControllers.Penitent.Attack.SwordAnimatorInyector>().GetComponent<Animator>().runtimeAnimatorController;
                }
                return m_SwordAnimator;
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
