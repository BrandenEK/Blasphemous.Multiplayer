using System;
using System.Collections.Generic;
using System.Text;
using SimpleTCP;

namespace BlasServer
{
    public class Server
    {
        private string ipAddress;
        private SimpleTcpServer server;

        private Dictionary<string, PlayerStatus> connectedPlayers;
        private string currentIp;

        public Server(string ip)
        {
            ipAddress = ip;
        }

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
                Core.displayMessage($"Sending {list.Count} bytes");
                server.Send(ip, list.ToArray());
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
                    default:
                        Core.displayError($"Data type '{type}' is not valid"); break;
                }
            }
        }

        private void clientConnected(object sender, ClientConnectionEventArgs e)
        {
            Core.displayMessage("Client connected at " + e.ip);
            connectedPlayers.Add(e.ip, new PlayerStatus());
            connectedPlayers[e.ip].name = connectedPlayers.Count.ToString(); // temp
        }

        private void clientDisconnected(object sender, ClientConnectionEventArgs e)
        {
            Core.displayMessage("Client disconnected at " + e.ip);
            connectedPlayers.Remove(e.ip);
            // Noitification for leave
        }

        private PlayerStatus getCurrentPlayer()
        {
            if (connectedPlayers.ContainsKey(currentIp))
                return connectedPlayers[currentIp];

            Core.displayError("Data for " + currentIp + " has not been created yet!");
            return new PlayerStatus();
        }

        private byte[] getPositionBytes(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(BitConverter.GetBytes(player.xPos));
            bytes.AddRange(BitConverter.GetBytes(player.yPos));
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
        public void sendPlayerPostition(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.sceneName == connectedPlayers[ip].sceneName)
                {
                    // Send this player's updated position
                    Send(ip, getPositionBytes(player), 0);
                }
            }
        }

        // Send a player's updated animation
        public void sendPlayerAnimation(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.sceneName == connectedPlayers[ip].sceneName)
                {
                    // Send this player's updated animation
                    List<byte> bytes = addPlayerNameToData(player.name);
                    bytes.Add(player.animation);
                    Send(ip, bytes.ToArray(), 1);
                }
            }
        }

        // Send that a player entered a scene
        public void sendPlayerEnterScene(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.sceneName == connectedPlayers[ip].sceneName)
                {
                    // Send that this player has entered their scene
                    Send(ip, Encoding.UTF8.GetBytes(player.name), 2);

                    // Send that the other player is in this player's scene & the other player's position/animation
                    Send(currentIp, Encoding.UTF8.GetBytes(connectedPlayers[ip].name), 2);
                    Send(currentIp, getPositionBytes(connectedPlayers[ip]), 0);
                    // Send animation data
                }
            }
        }

        // Send that a player left a scene
        public void sendPlayerLeaveScene(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.sceneName == connectedPlayers[ip].sceneName)
                {
                    // Send that this player has left their scene
                    Send(ip, Encoding.UTF8.GetBytes(player.name), 3);
                }
            }
        }

        // Send a player's updated direction
        public void sendPlayerDirection(PlayerStatus player)
        {
            foreach (string ip in connectedPlayers.Keys)
            {
                if (currentIp != ip && player.sceneName == connectedPlayers[ip].sceneName)
                {
                    // Send this player's updated direction
                    List<byte> bytes = addPlayerNameToData(player.name);
                    bytes.AddRange(BitConverter.GetBytes(player.facingDirection));
                    Send(ip, bytes.ToArray(), 4);
                }
            }
        }

        #endregion Send functions

        #region Receive functions

        // Received a player's updated position
        public void receivePlayerPostition(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.xPos = BitConverter.ToSingle(data, 0);
            current.yPos = BitConverter.ToSingle(data, 4);
            current.facingDirection = BitConverter.ToBoolean(data, 8);

            sendPlayerPostition(current);
        }

        // Recieved a player's updated animation
        public void receivePlayerAnimation(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.animation = data[0];

            sendPlayerAnimation(current);
        }

        // Received that a player entered a scene
        public void receivePlayerEnterScene(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.sceneName = Encoding.UTF8.GetString(data);

            sendPlayerEnterScene(current);
        }

        // Received that a player left a scene
        public void receivePlayerLeaveScene(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();

            sendPlayerLeaveScene(current);
            current.sceneName = "";
        }

        // Recieved a player's updated direction
        public void receivePlayerDirection(byte[] data)
        {
            PlayerStatus current = getCurrentPlayer();
            current.facingDirection = BitConverter.ToBoolean(data);

            sendPlayerDirection(current);
        }

        // Right after client connects, they send their name
        void receivePlayerName(byte[] data)
        {
            string name = Encoding.UTF8.GetString(data);
            connectedPlayers[currentIp].name = name;
            // Notification for join
        }

        #endregion Receive functions
    }
}
