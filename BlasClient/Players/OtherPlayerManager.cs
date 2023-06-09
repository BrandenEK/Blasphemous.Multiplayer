using BlasClient.MonoBehaviours;
using Framework.Managers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UI;

namespace BlasClient.Players
{
    public class OtherPlayerManager
    {
        private readonly List<PlayerStatus> connectedPlayers = new();
        public ReadOnlyCollection<PlayerStatus> AllConnectedPlayers => connectedPlayers.AsReadOnly();

        private readonly List<OtherPlayerScript> activePlayers = new();
        public ReadOnlyCollection<OtherPlayerScript> AllActivePlayers => activePlayers.AsReadOnly();


        #region Connected players

        // Finds a specified player connected to the server
        public PlayerStatus FindConnectedPlayer(string name)
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                if (connectedPlayers[i].Name == name)
                    return connectedPlayers[i];
            }

            Main.Multiplayer.LogWarning("Couldn't find connected player: " + name);
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

            Main.Multiplayer.LogWarning("Couldn't find active player: " + name);
            return null;
        }

        // When a player enters a scene, create a new player object
        public void AddActivePlayer(string name)
        {
            // Create & setup new penitent object
            GameObject playerObject = new GameObject("_" + name);
            OtherPlayerScript player = playerObject.AddComponent<OtherPlayerScript>();
            player.createPenitent(name, storedPenitentAnimator, storedSwordAnimator, storedPenitentMaterial);
            activePlayers.Add(player);

            // If in beginning room, add fake penitent controller
            if (Core.LevelManager.currentLevel.LevelName == "D17Z01S01")
                playerObject.AddComponent<FakePenitentIntro>();

            // Set up name tag
            if (Main.Multiplayer.config.displayNametags)
                createNameTag(name, Main.Multiplayer.playerList.getPlayerTeam(name) == Main.Multiplayer.PlayerTeam);

            Main.Multiplayer.Log("Created new player object for " + name);
        }

        // When a player leaves a scene, destroy the player object
        public void RemoveActivePlayer(string name)
        {
            OtherPlayerScript player = FindActivePlayer(name);
            if (player != null)
            {
                activePlayers.Remove(player);
                Object.Destroy(player.gameObject);
                Main.Multiplayer.Log("Removed player object for " + name);
            }

            Text nametag = getPlayerNametag(name);
            if (nametag != null)
            {
                nametags.Remove(nametag);
                Object.Destroy(nametag);
                Main.Multiplayer.Log("Removed nametag for " + name);
            }
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
            for (int i = 0; i < nametags.Count; i++)
            {
                if (nametags[i] != null)
                    Object.Destroy(nametags[i].gameObject);
            }
            nametags.Clear();
        }

        #endregion Active players

        #region Receive updates

        public void ReceivePosition(string playerName, Vector2 position)
        {
            if (!Main.Multiplayer.CurrentlyInLevel) return;

            OtherPlayerScript player = FindActivePlayer(playerName);
            if (player != null)
            {
                player.updatePosition(position);
            }
        }

        public void ReceiveAnimation(string playerName, byte animation)
        {
            if (!Main.Multiplayer.CurrentlyInLevel) return;

            OtherPlayerScript player = FindActivePlayer(playerName);
            if (player != null)
            {
                player.updateAnimation(animation);
            }
        }

        public void ReceiveDirection(string playerName, bool direction)
        {
            if (!Main.Multiplayer.CurrentlyInLevel) return;

            OtherPlayerScript player = FindActivePlayer(playerName);
            if (player != null)
            {
                player.updateDirection(direction);
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

        #endregion Receive updates
    }
}
