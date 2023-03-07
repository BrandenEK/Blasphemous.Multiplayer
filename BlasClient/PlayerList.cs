using System.Collections.Generic;
using UnityEngine;
using BlasClient.Structures;

namespace BlasClient
{
    public class PlayerList
    {
        private Dictionary<string, PlayerStatus> connectedPlayers;
        private static readonly object playerListLock = new object();

        // List methods

        public void AddPlayer(string name)
        {
            lock (playerListLock)
            {
                if (!connectedPlayers.ContainsKey(name))
                    connectedPlayers.Add(name, new PlayerStatus());
            }
        }

        public void RemovePlayer(string name)
        {
            lock (playerListLock)
            {
                if (connectedPlayers.ContainsKey(name))
                    connectedPlayers.Remove(name);
            }
        }

        public void ClearPlayers()
        {
            lock (playerListLock)
            {
                connectedPlayers.Clear();
            }
        }

        // Data methods

        private PlayerStatus getPlayerStatus(string playerName)
        {
            if (connectedPlayers.ContainsKey(playerName))
                return connectedPlayers[playerName];

            Main.Multiplayer.LogWarning("Error: Player is not in the server: " + playerName);
            return new PlayerStatus();
        }

        public IEnumerable<string> getAllPlayers()
        {
            lock (playerListLock)
            {
                return connectedPlayers.Keys;
            }
        }

        public void setPlayerSkinTexture(string name, byte[] texture)
        {
            lock (playerListLock)
            {
                getPlayerStatus(name).skin.createSkin(texture);
            }
        }

        public Texture2D getPlayerSkinTexture(string name)
        {
            lock (playerListLock)
            {
                return getPlayerStatus(name).skin.skinTexture;
            }
        }

        public void setPlayerSkinUpdateStatus(string name, byte status)
        {
            lock (playerListLock)
            {
                getPlayerStatus(name).skin.updateStatus = status;
            }
        }

        public byte getPlayerSkinUpdateStatus(string name)
        {
            lock (playerListLock)
            {
                return getPlayerStatus(name).skin.updateStatus;
            }
        }

        public void setPlayerTeam(string name, byte team)
        {
            lock (playerListLock)
            {
                getPlayerStatus(name).team = team;
            }
        }

        public byte getPlayerTeam(string name)
        {
            lock (playerListLock)
            {
                return getPlayerStatus(name).team;
            }
        }

        public void setPlayerSpecialAnimation(string name, byte animation)
        {
            lock (playerListLock)
            {
                getPlayerStatus(name).specialAnimation = animation;
            }
        }

        public byte getPlayerSpecialAnimation(string name)
        {
            lock (playerListLock)
            {
                return getPlayerStatus(name).specialAnimation;
            }
        }

        public void setPlayerScene(string name, string scene)
        {
            lock (playerListLock)
            {
                PlayerStatus playerStatus = getPlayerStatus(name);
                playerStatus.currentScene = scene;
                if (scene.Length == 9)
                    playerStatus.lastMapScene = scene;
            }
        }

        public string getPlayerScene(string name)
        {
            lock (playerListLock)
            {
                return getPlayerStatus(name).currentScene;
            }
        }

        public string getPlayerMapScene(string name)
        {
            lock (playerListLock)
            {
                return getPlayerStatus(name).lastMapScene;
            }
        }

        public PlayerList()
        {
            connectedPlayers = new Dictionary<string, PlayerStatus>();
        }
    }
}
