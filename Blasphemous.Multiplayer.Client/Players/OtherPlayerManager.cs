using Blasphemous.ModdingAPI;
using Framework.Managers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tools.Level;
using Tools.Level.Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.Multiplayer.Client.Players
{
    public class OtherPlayerManager
    {
        private readonly List<PlayerStatus> connectedPlayers = new();
        public ReadOnlyCollection<PlayerStatus> AllConnectedPlayers => connectedPlayers.AsReadOnly();

        private readonly List<OtherPlayerScript> activePlayers = new();
        public ReadOnlyCollection<OtherPlayerScript> AllActivePlayers => activePlayers.AsReadOnly();

        private readonly List<Text> nametags = new ();
        public ReadOnlyCollection<Text> AllNametags => nametags.AsReadOnly();


        public void LevelLoaded(string scene)
        {
            // Remove all existing player objects and nametags
            RemoveAllActivePlayers();

            // Create any players that are already in this scene
            foreach (PlayerStatus player in connectedPlayers)
            {
                if (player.CurrentScene == scene)
                    AddActivePlayer(player.Name);
            }

            // Add special animation checkers to certain interactors
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

            // Add this to the fake penitent intro animator
            if (scene == "D17Z01S01")
            {
                GameObject fakePenitent = GameObject.Find("FakePenitent");
                if (fakePenitent != null)
                {
                    fakePenitent.AddComponent<SpecialAnimationChecker>();
                    count++;
                }
            }
            ModLog.Info("Adding special animation checkers to " + count + " objects!");

            // Create main player's nametag
            if (Main.Multiplayer.NetworkManager.IsConnected)
                AddNametag(Main.Multiplayer.PlayerName, true);
        }

        public void Update()
        {
            // Check status of player skins and potentially update the textures
            foreach (PlayerStatus player in connectedPlayers)
            {
                PlayerStatus.SkinStatus currentSkinStatus = player.SkinUpdateStatus;
                if (currentSkinStatus == PlayerStatus.SkinStatus.NoUpdate)
                {
                    // Set that one update cycle has passed
                    player.SkinUpdateStatus = PlayerStatus.SkinStatus.YesUpdate;
                }
                else if (currentSkinStatus == PlayerStatus.SkinStatus.YesUpdate)
                {
                    // Set the player texture
                    ApplySkinTexture(player.Name);
                    player.SkinUpdateStatus = PlayerStatus.SkinStatus.Updated;
                }
            }

            // Update position of all name tags
            for (int i = 0; i < nametags.Count; i++)
            {
                RectTransform nametag = nametags[i].rectTransform;
                string name = nametags[i].name;

                // Get player with this name
                Vector3 position;
                if (name == Main.Multiplayer.PlayerName)
                {
                    position = Core.Logic.Penitent.transform.position;
                }
                else
                {
                    OtherPlayerScript player = FindActivePlayer(name);
                    if (player != null)
                        position = player.transform.position;
                    else
                        continue;
                }

                Vector3 viewPosition = Camera.main.WorldToViewportPoint(position + Vector3.up * 3.1f);
                nametag.anchorMin = viewPosition;
                nametag.anchorMax = viewPosition;
                nametag.anchoredPosition = Vector2.zero;
            }
        }

        // Sets the skin texture of a player's object - must be delayed until after object creation
        private void ApplySkinTexture(string playerName)
        {
            // Get player object with this name
            OtherPlayerScript activePlayer = FindActivePlayer(playerName);
            if (activePlayer == null) return;

            ModLog.Info("Setting skin texture for " + playerName);
            activePlayer.ApplySkinTexture();
        }

        #region Connected players

        // Finds a specified player connected to the server
        public PlayerStatus FindConnectedPlayer(string name)
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                if (connectedPlayers[i].Name == name)
                    return connectedPlayers[i];
            }

            ModLog.Warn("Couldn't find connected player: " + name);
            return null;
        }

        public void AddConnectedPlayer(string name)
        {
            connectedPlayers.Add(new PlayerStatus(name));
        }

        public void RemoveConnectedPlayer(string name)
        {
            PlayerStatus player = FindConnectedPlayer(name);
            if (player != null)
            {
                connectedPlayers.Remove(player);
            }
        }

        public void RemoveAllConnectedPlayers()
        {
            connectedPlayers.Clear();
        }

        #endregion Connected players

        #region Active players

        // Finds a specified player in the scene
        public OtherPlayerScript FindActivePlayer(string name)
        {
            for (int i = 0; i < activePlayers.Count; i++)
            {
                if (activePlayers[i].name == "_" + name)
                    return activePlayers[i];
            }

            ModLog.Warn("Couldn't find active player: " + name);
            return null;
        }

        // When a player enters a scene, create a new player object
        public void AddActivePlayer(string name)
        {
            PlayerStatus player = FindConnectedPlayer(name);
            if (player == null) return;

            // Create & setup new penitent object
            GameObject playerObject = new GameObject("_" + name);
            OtherPlayerScript activePlayer = playerObject.AddComponent<OtherPlayerScript>();
            activePlayer.SetupPlayer(player);
            activePlayers.Add(activePlayer);

            // If in beginning room, add fake penitent controller
            if (Core.LevelManager.currentLevel.LevelName == "D17Z01S01")
                playerObject.AddComponent<FakePenitentIntro>();

            // Set up name tag
            AddNametag(name, Main.Multiplayer.PlayerTeam == player.Team);

            ModLog.Info("Created new player object for " + name);
        }

        // When a player leaves a scene, destroy the player object
        public void RemoveActivePlayer(string name)
        {
            OtherPlayerScript player = FindActivePlayer(name);
            if (player != null)
            {
                activePlayers.Remove(player);
                Object.Destroy(player.gameObject);
                ModLog.Info("Removed player object for " + name);
            }

            RemoveNametag(name);
        }

        // When disconnected from server or loading new scene, remove all players
        public void RemoveAllActivePlayers()
        {
            for (int i = 0; i < activePlayers.Count; i++)
            {
                if (activePlayers[i] != null)
                    Object.Destroy(activePlayers[i].gameObject);
            }
            activePlayers.Clear();

            RemoveAllNametags();
        }

        #endregion Active players

        #region Nametags

        public Text FindNametag(string name)
        {
            for (int i = 0; i < nametags.Count; i++)
            {
                if (nametags[i].name == name)
                    return nametags[i];
            }

            ModLog.Warn("Couldn't find nametag: " + name);
            return null;
        }

        public void AddNametag(string name, bool friendlyTeam)
        {
            if (!Main.Multiplayer.config.displayNametags || !Main.Multiplayer.config.displayOwnNametag && name == Main.Multiplayer.PlayerName)
                return;

            Transform parent = UnityReferences.CanvasObject;
            GameObject text = UnityReferences.TextObject;

            if (parent == null || text == null)
            {
                ModLog.Error("Error: Failed to create nametag for " + name);
                return;
            }

            // Could probably use the UI framework for this...
            Text nametag = Object.Instantiate(text, parent).GetComponent<Text>();
            nametag.rectTransform.sizeDelta = new Vector2(100, 50);
            nametag.rectTransform.SetAsFirstSibling();
            nametag.name = name;
            nametag.text = name;
            nametag.alignment = TextAnchor.LowerCenter;
            nametag.color = friendlyTeam ? new Color(0.671f, 0.604f, 0.247f) : Color.red;
            nametags.Add(nametag);
        }

        public void RemoveNametag(string name)
        {
            Text nametag = FindNametag(name);
            if (nametag != null)
            {
                nametags.Remove(nametag);
                Object.Destroy(nametag.gameObject);
                ModLog.Info("Removed nametag for " + name);
            }
        }

        public void RemoveAllNametags()
        {
            for (int i = 0; i < nametags.Count; i++)
            {
                if (nametags[i] != null)
                    Object.Destroy(nametags[i].gameObject);
            }
            nametags.Clear();
        }

        // Updates the colors of all nametags in the scene when someone changes teams
        public void RefreshNametagColors()
        {
            for (int i = 0; i < nametags.Count; i++)
            {
                string name = nametags[i].name;
                bool friendlyTeam;

                if (name == Main.Multiplayer.PlayerName)
                {
                    friendlyTeam = true;
                }
                else
                {
                    PlayerStatus player = FindConnectedPlayer(name);
                    if (player != null)
                        friendlyTeam = Main.Multiplayer.PlayerTeam == player.Team;
                    else
                        continue;
                }
                
                nametags[i].GetComponent<Text>().color = friendlyTeam ? new Color(0.671f, 0.604f, 0.247f) : Color.red;
            }
        }

        #endregion Nametags

        #region Receive updates

        public void ReceivePosition(string playerName, Vector2 position)
        {
            if (!Main.Multiplayer.CurrentlyInLevel) return;

            OtherPlayerScript player = FindActivePlayer(playerName);
            if (player != null)
            {
                player.CurrentPosition = position;
            }
        }

        public void ReceiveAnimation(string playerName, byte animation)
        {
            if (!Main.Multiplayer.CurrentlyInLevel) return;

            OtherPlayerScript player = FindActivePlayer(playerName);
            if (player != null)
            {
                player.CurrentAnimation = animation;
            }
        }

        public void ReceiveDirection(string playerName, bool direction)
        {
            if (!Main.Multiplayer.CurrentlyInLevel) return;

            OtherPlayerScript player = FindActivePlayer(playerName);
            if (player != null)
            {
                player.CurrentDirection = direction;
            }
        }

        public void ReceiveEnterScene(string playerName, string scene)
        {
            PlayerStatus player = FindConnectedPlayer(playerName);
            if (player == null) return;

            player.CurrentScene = scene;
            if (Core.LevelManager.currentLevel.LevelName == scene)
                AddActivePlayer(playerName);

            Main.Multiplayer.MapManager.QueueMapUpdate();
        }

        public void ReceiveLeaveScene(string playerName)
        {
            PlayerStatus player = FindConnectedPlayer(playerName);
            if (player == null) return;

            if (Core.LevelManager.currentLevel.LevelName == player.CurrentScene)
                RemoveActivePlayer(playerName);
            player.CurrentScene = string.Empty;

            Main.Multiplayer.MapManager.QueueMapUpdate();
        }

        public void ReceiveTeam(string playerName, byte team)
        {
            PlayerStatus player = FindConnectedPlayer(playerName);
            if (player == null) return;

            player.Team = team;
            if (Main.Multiplayer.CurrentlyInLevel)
                Main.Multiplayer.RefreshPlayerColors();
        }

        public void ReceivePing(string playerName, ushort ping)
        {
            PlayerStatus player = FindConnectedPlayer(playerName);
            if (player == null) return;

            player.Ping = ping;
        }

        #endregion Receive updates
    }
}
