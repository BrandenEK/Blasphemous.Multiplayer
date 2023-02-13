using System;
using System.Collections.Generic;
using System.Text;
using SimpleTCP;

namespace BlasServer
{
    public class Server
    {
        private SimpleTcpServer server;

        private Dictionary<string, PlayerStatus> connectedPlayers;

        public bool Start()
        {
            try
            {
                server = new SimpleTcpServer();
                server.ClientConnected += clientConnected;
                server.ClientDisconnected += clientDisconnected;
                server.DataReceived += Receive;
                server.Start(Core.config.serverPort);
                server.DisableDelay(); // Does this even work ??
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }

            connectedPlayers = new Dictionary<string, PlayerStatus>();
            return true;
        }

        private void Send(string ip, byte[] data, byte dataType)
        {
            if (data != null && data.Length > 0)
            {
                List<byte> list = new List<byte>(BitConverter.GetBytes((ushort)data.Length));
                list.Add(dataType);
                list.AddRange(data);

                try
                {
                    //Core.displayMessage($"Sending {list.Count} bytes");
                    server.Send(ip, list.ToArray());
                }
                catch (Exception)
                {
                    Core.displayError("Couldn't send data to " + ip + "!");
                }
            }
        }

        // Data should be formatted as length length type data
        private void Receive(object sender, DataReceivedEventArgs e)
        {
            //Core.displayMessage("Bytes received: " + e.data.Length);

            int startIdx = 0;
            while (startIdx < e.data.Length - 3)
            {
                ushort length = BitConverter.ToUInt16(e.data, startIdx);
                byte type = e.data[startIdx + 2];
                byte[] messageData = e.data[(startIdx + 3)..(startIdx + 3 + length)];

                processDataReceived(type, messageData);
                startIdx += 3 + length;
            }
            if (startIdx != e.data.Length)
                Core.displayError("Received data was formatted incorrectly");

            // Determines which received function to call based on the type
            void processDataReceived(byte type, byte[] data)
            {
                switch (type)
                {
                    case 0:
                        receivePlayerPostition(e.ip, data); break;
                    case 1:
                        receivePlayerAnimation(e.ip, data); break;
                    case 2:
                        receivePlayerEnterScene(e.ip, data); break;
                    case 3:
                        receivePlayerLeaveScene(e.ip, data); break;
                    case 4:
                        receivePlayerDirection(e.ip, data); break;
                    case 5:
                        receivePlayerSkin(e.ip, data); break;
                    case 6:
                        receivePlayerIntro(e.ip, data); break;
                    case 8:
                        receivePlayerProgress(e.ip, data); break;
                    case 9:
                        receivePlayerTeam(e.ip, data); break;
                    default:
                        Core.displayError($"Data type '{type}' is not valid"); break;
                }
            }
        }

        private void clientConnected(object sender, ClientConnectionEventArgs e)
        {
            Core.displayMessage("Client connected at " + e.ip);
        }

        private void clientDisconnected(object sender, ClientConnectionEventArgs e)
        {
            Core.displayMessage("Client disconnected at " + e.ip);

            // Make sure this client was actually connected, not just rejected from server
            if (!connectedPlayers.ContainsKey(e.ip))
                return;

            // Send that this player has disconnected
            sendPlayerConnection(e.ip, false);

            // Remove this player from connected list
            connectedPlayers.Remove(e.ip);
        }

        private PlayerStatus getCurrentPlayer(string ip)
        {
            if (connectedPlayers.ContainsKey(ip))
                return connectedPlayers[ip];

            Core.displayError("Data for " + ip + " has not been created yet!");
            return new PlayerStatus("");
        }

        private byte[] getPositionPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(BitConverter.GetBytes(player.xPos));
            bytes.AddRange(BitConverter.GetBytes(player.yPos));
            return bytes.ToArray();
        }
        private byte[] getAnimationPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.Add(player.animation);
            return bytes.ToArray();
        }
        private byte[] getDirectionPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(BitConverter.GetBytes(player.facingDirection));
            return bytes.ToArray();
        }
        private byte[] getSkinPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(Encoding.UTF8.GetBytes(player.skin));
            return bytes.ToArray();
        }
        private byte[] getScenePacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(Encoding.UTF8.GetBytes(player.sceneName));
            return bytes.ToArray();
        }
        private byte[] getIntroPacket(byte response)
        {
            return new byte[] { response };
        }
        private byte[] getConnectionPacket(PlayerStatus player, bool connected)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(BitConverter.GetBytes(connected));
            return bytes.ToArray();
        }
        private byte[] getProgressPacket(string nameOrStar, byte type, byte value, string id)
        {
            List<byte> bytes = addPlayerNameToData(nameOrStar);
            bytes.Add(type);
            bytes.Add(value);
            bytes.AddRange(Encoding.UTF8.GetBytes(id));
            return bytes.ToArray();
        }
        private byte[] getTeamPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.Add(player.team);
            return bytes.ToArray();
        }

        private List<byte> addPlayerNameToData(string name)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            List<byte> data = new List<byte>();
            data.Add((byte)nameBytes.Length);
            data.AddRange(nameBytes);
            return data;
        }

        #region Send functions

        // Send a player's updated position
        private void sendPlayerPostition(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated position
                    Send(ip, getPositionPacket(current), 0);
                }
            }
        }

        // Send a player's updated animation
        private void sendPlayerAnimation(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated animation
                    Send(ip, getAnimationPacket(current), 1);
                }
            }
        }

        // Send that a player entered a scene
        private void sendPlayerEnterScene(string playerIp) // Optimize these send functions to combine them together
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    // Send that this player has entered a new scene
                    Send(ip, getScenePacket(current), 2);

                    if (current.isInSameScene(connectedPlayers[ip]))
                    {
                        // If in same scene, also send position data to each one
                        Send(ip, getPositionPacket(current), 0);
                        Send(ip, getAnimationPacket(current), 1);
                        Send(ip, getDirectionPacket(current), 4);

                        Send(playerIp, getPositionPacket(connectedPlayers[ip]), 0);
                        Send(playerIp, getAnimationPacket(connectedPlayers[ip]), 1);
                        Send(playerIp, getDirectionPacket(connectedPlayers[ip]), 4);
                    }
                }
            }
        }

        // Send that a player left a scene
        private void sendPlayerLeaveScene(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.xPos = 0;
            current.yPos = 0;
            current.animation = 0;
            current.facingDirection = false;

            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    // Send that this player has left their old scene
                    Send(ip, Encoding.UTF8.GetBytes(current.name), 3);
                }
            }
        }

        // Send a player's updated direction
        private void sendPlayerDirection(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated direction
                    Send(ip, getDirectionPacket(current), 4);
                }
            }
        }

        // Send a player's updated skin
        private void sendPlayerSkin(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    // Send this player's updated skin
                    Send(ip, getSkinPacket(current), 5);
                }
            }
        }

        // Send a player's intro response
        private void sendPlayerIntro(string playerIp, byte response)
        {
            Send(playerIp, getIntroPacket(response), 6);

            // Only send rest of data if successful connection
            if (response > 0)
                return;

            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    // Send all other connected players and their important data
                    Send(playerIp, getConnectionPacket(connectedPlayers[ip], true), 7);
                    Send(playerIp, getSkinPacket(connectedPlayers[ip]), 5);
                    if (connectedPlayers[ip].sceneName != "")
                    {
                        Send(playerIp, getScenePacket(connectedPlayers[ip]), 2);
                    }
                    // Maybe send oter player's teams also
                }
            }

            // Send all of the server data to this player for them to merge
            Core.displayMessage("Sending all server data to " + playerIp);
            for (byte i = 0; i < Core.gameData.numberOfProgressTypes; i++)
            {
                Dictionary<string, byte> progressSet = Core.gameData.getProgressSet(i);
                foreach (string id in progressSet.Keys)
                {
                    Send(playerIp, getProgressPacket("*", i, progressSet[id], id), 8);
                }
            }
        }

        // Send that a player connected or disconnected
        private void sendPlayerConnection(string playerIp, bool connected)
        {
            // Send message to all other connected ips
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    Send(ip, getConnectionPacket(current, connected), 7);
                }
            }
        }

        // Send a player progress update
        private void sendPlayerProgress(string playerIp, byte type, byte value, string id)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    Send(ip, getProgressPacket(current.name, type, value, id), 8);
                }
            }
        }

        // Send a player team update
        private void sendPlayerTeam(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    Send(ip, getTeamPacket(current), 9);
                }
            }
        }

        #endregion Send functions

        #region Receive functions

        // Received a player's updated position
        private void receivePlayerPostition(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.xPos = BitConverter.ToSingle(data, 0);
            current.yPos = BitConverter.ToSingle(data, 4);

            sendPlayerPostition(playerIp);
        }

        // Recieved a player's updated animation
        private void receivePlayerAnimation(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.animation = data[0];

            sendPlayerAnimation(playerIp);
        }

        // Received that a player entered a scene
        private void receivePlayerEnterScene(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.sceneName = Encoding.UTF8.GetString(data);

            sendPlayerEnterScene(playerIp);
        }

        // Received that a player left a scene
        private void receivePlayerLeaveScene(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);

            sendPlayerLeaveScene(playerIp);
            current.sceneName = "";
        }

        // Recieved a player's updated direction
        private void receivePlayerDirection(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.facingDirection = BitConverter.ToBoolean(data);

            sendPlayerDirection(playerIp);
        }

        // Received a player's updated skin
        private void receivePlayerSkin(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.skin = Encoding.UTF8.GetString(data);

            sendPlayerSkin(playerIp);
        }

        // Received a player's introductory data
        private void receivePlayerIntro(string playerIp, byte[] data)
        {
            // Ensure there are no duplicate names
            string playerName = Encoding.UTF8.GetString(data);
            foreach (PlayerStatus player in connectedPlayers.Values)
            {
                if (player.name == playerName)
                {
                    Core.displayMessage("Player connection rejected: Duplicate name");
                    sendPlayerIntro(playerIp, 1);
                    return;
                }
            }

            // Ensure the server doesn't have max number of players
            if (connectedPlayers.Count >= Core.config.maxPlayers)
            {
                Core.displayMessage("Player connection rejected: Player limit reached");
                sendPlayerIntro(playerIp, 2);
                return;
            }

            // Ensure this ip address is not banned


            // Add new connected player
            Core.displayMessage("Player connection accepted");
            PlayerStatus newPlayer = new PlayerStatus(playerName);
            connectedPlayers.Add(playerIp, newPlayer);
            sendPlayerConnection(playerIp, true);
            sendPlayerIntro(playerIp, 0);
        }

        // Received a player progress update
        private void receivePlayerProgress(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            byte progressType = data[0];
            byte progressValue = data[1];
            string progressId = Encoding.UTF8.GetString(data, 2, data.Length - 2);

            // Add the progress to the server data, and if it's new send it to the rest of the players
            if (!Core.gameData.addPlayerProgress(progressId, progressType, progressValue))
            {
                Core.displayCustom($"Received duplicated or inferior progress from {current.name}: {progressId}, Type {progressType}, Value {progressValue}", ConsoleColor.DarkGreen);
                return;
            }

            if (progressType >= 0 && progressType <= 5)
            {
                // Item
                Core.displayCustom($"{(progressValue == 0 ? "Received new" : "Lost an")} item from {current.name}: {progressId}", ConsoleColor.Green);
            }
            else if (progressType == 6)
            {
                // Stat
                Core.displayCustom($"Received new stat upgrade from {current.name}: {progressId} level {progressValue + 1}", ConsoleColor.Green);
            }
            else if (progressType == 7)
            {
                // Skill
                Core.displayCustom($"Received new skill from {current.name}: {progressId}", ConsoleColor.Green);
            }
            else if (progressType == 8)
            {
                // Map cell
                Core.displayCustom($"Received new map cell from {current.name}: {progressId}", ConsoleColor.Green);
            }
            else if (progressType == 9)
            {
                // Flag
                Core.displayCustom($"Received new flag from {current.name}: {progressId}", ConsoleColor.Green);
            }
            else if (progressType == 10)
            {
                // Pers. object
                Core.displayCustom($"Received new pers. object from {current.name}: {progressId}", ConsoleColor.Green);
            }
            else if (progressType == 11)
            {
                // Teleport
                Core.displayCustom($"Received new teleport location from {current.name}: {progressId}", ConsoleColor.Green);
            }
            
            sendPlayerProgress(playerIp, progressType, progressValue, progressId);
        }

        // Received a player's new team
        private void receivePlayerTeam(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.team = data[0];

            sendPlayerTeam(playerIp);
        }

        #endregion Receive functions
    }
}
