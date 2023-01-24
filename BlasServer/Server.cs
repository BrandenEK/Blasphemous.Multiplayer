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
        private string currentIp;

        public bool Start()
        {
            try
            {
                server = new SimpleTcpServer();
                server.ClientConnected += clientConnected;
                server.ClientDisconnected += clientDisconnected;
                server.DataReceived += Receive;
                server.Start(25565);
                server.DisableDelay();
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
                    Core.displayMessage($"Sending {list.Count} bytes");
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
            Core.displayMessage("Bytes received: " + e.data.Length);
            currentIp = e.ip;

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
                        receivePlayerPostition(data); break;
                    case 1:
                        receivePlayerAnimation(data); break;
                    case 2:
                        receivePlayerEnterScene(data); break;
                    case 3:
                        receivePlayerLeaveScene(data); break;
                    case 4:
                        receivePlayerDirection(data); break;
                    case 5:
                        receivePlayerSkin(data); break;
                    case 6:
                        receivePlayerIntro(data); break;
                    default:
                        Core.displayError($"Data type '{type}' is not valid"); break;
                }
            }
        }

        private void clientConnected(object sender, ClientConnectionEventArgs e)
        {
            Core.displayMessage("Client connected at " + e.ip);
            currentIp = e.ip;
        }

        private void clientDisconnected(object sender, ClientConnectionEventArgs e)
        {
            Core.displayMessage("Client disconnected at " + e.ip);
            currentIp = e.ip;

            // For now, just send playerLeft packet to remove the player from the scene
            // Later will need a special packet to also remove the player from the client's list
            // Client's skin list will currently still keep this player in it
            PlayerStatus player = getCurrentPlayer();
            sendPlayerLeaveScene(player);
            // Send disconnect notification to other players

            // Remove this player from connected list
            connectedPlayers.Remove(e.ip);
        }

        private PlayerStatus getCurrentPlayer()
        {
            if (connectedPlayers.ContainsKey(currentIp))
                return connectedPlayers[currentIp];

            Core.displayError("Data for " + currentIp + " has not been created yet!");
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
        private byte[] getIntroPacket(byte response)
        {
            return new byte[] { response };
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
        private void sendPlayerPostition(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated position
                    Send(ip, getPositionPacket(player), 0);
                }
            }
        }

        // Send a player's updated animation
        private void sendPlayerAnimation(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated animation
                    Send(ip, getAnimationPacket(player), 1);
                }
            }
        }

        // Send that a player entered a scene
        private void sendPlayerEnterScene(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.isInSameScene(connectedPlayers[ip]))
                {
                    // Send that this player has entered their scene & this player's position/animation/direction
                    Send(ip, Encoding.UTF8.GetBytes(player.name), 2);
                    Send(ip, getPositionPacket(player), 0);
                    Send(ip, getAnimationPacket(player), 1);
                    Send(ip, getDirectionPacket(player), 4);

                    // Send that the other player is in this player's scene & the other player's position/animation/direction
                    Send(currentIp, Encoding.UTF8.GetBytes(connectedPlayers[ip].name), 2);
                    Send(currentIp, getPositionPacket(connectedPlayers[ip]), 0);
                    Send(currentIp, getAnimationPacket(connectedPlayers[ip]), 1);
                    Send(currentIp, getDirectionPacket(connectedPlayers[ip]), 4);
                }
            }
        }

        // Send that a player left a scene
        private void sendPlayerLeaveScene(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.isInSameScene(connectedPlayers[ip]))
                {
                    // Send that this player has left their scene
                    Send(ip, Encoding.UTF8.GetBytes(player.name), 3);
                }
            }
        }

        // Send a player's updated direction
        private void sendPlayerDirection(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated direction
                    Send(ip, getDirectionPacket(player), 4);
                }
            }
        }

        // Send a player's updated skin
        private void sendPlayerSkin(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip)
                {
                    // Send this player's updated skin
                    Send(ip, getSkinPacket(player), 5);
                }
            }
        }

        // Send a player's intro response
        private void sendPlayerIntro(byte response)
        {
            Send(currentIp, getIntroPacket(response), 6);

            // Only send rest of data if successful connection
            if (response > 0)
                return;

            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip)
                {
                    Send(currentIp, getSkinPacket(connectedPlayers[ip]), 5);
                }
            }
        }

        #endregion Send functions

        #region Receive functions

        // Received a player's updated position
        private void receivePlayerPostition(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.xPos = BitConverter.ToSingle(data, 0);
            current.yPos = BitConverter.ToSingle(data, 4);

            sendPlayerPostition(current);
        }

        // Recieved a player's updated animation
        private void receivePlayerAnimation(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.animation = data[0];

            sendPlayerAnimation(current);
        }

        // Received that a player entered a scene
        private void receivePlayerEnterScene(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.sceneName = Encoding.UTF8.GetString(data);

            sendPlayerEnterScene(current);
        }

        // Received that a player left a scene
        private void receivePlayerLeaveScene(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();

            sendPlayerLeaveScene(current);
            current.sceneName = "";
        }

        // Recieved a player's updated direction
        private void receivePlayerDirection(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.facingDirection = BitConverter.ToBoolean(data);

            sendPlayerDirection(current);
        }

        // Received a player's updated skin
        private void receivePlayerSkin(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.skin = Encoding.UTF8.GetString(data);

            sendPlayerSkin(current);
        }

        // Received a player's introductory data
        private void receivePlayerIntro(byte[] data)
        {
            // Ensure there are no duplicate names
            string playerName = Encoding.UTF8.GetString(data);
            foreach (PlayerStatus player in connectedPlayers.Values)
            {
                if (player.name == playerName)
                {
                    Core.displayMessage("Player connection rejected: Duplicate name");
                    sendPlayerIntro(1);
                    return;
                }
            }

            // Ensure the server doesn't have max number of players
            if (connectedPlayers.Count >= Core.maxPlayers)
            {
                Core.displayMessage("Player connection rejected: Player limit reached");
                sendPlayerIntro(2);
                return;
            }

            // Ensure this ip address is not banned


            // Add new connected player
            Core.displayMessage("Player connection accepted");
            PlayerStatus newPlayer = new PlayerStatus(playerName);
            connectedPlayers.Add(currentIp, newPlayer);
            sendPlayerIntro(0);

            // Notification for joining
        } 

        #endregion Receive functions
    }
}
