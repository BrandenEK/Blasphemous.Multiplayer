using System;
using System.Collections.Generic;
using System.Text;
using SimpleTCP;

namespace BlasClient
{
    public class Client
    {
        public enum ConnectionStatus { Disconnnected, Attempting, Connected }
        public ConnectionStatus connectionStatus { get; private set; }
        public string serverIp { get; private set; }
        private SimpleTcpClient client;

        private List<byte> queuedMessages = new List<byte>();

        public Client()
        {
            // Start out as disconnected
            connectionStatus = ConnectionStatus.Disconnnected;
            serverIp = string.Empty;
        }

        public bool Connect(string playerName, string ipAddress)
        {
            if (connectionStatus != ConnectionStatus.Disconnnected) return false;

            try
            {
                client = new SimpleTcpClient();
                client.Connect(ipAddress, 25565);
                client.DataReceived += Receive;
                client.TcpClient.NoDelay = true;
                connectionStatus = ConnectionStatus.Attempting;
                serverIp = ipAddress;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }

            sendPlayerIntro(playerName);
            return true;
        }

        public void Disconnect()
        {
            Main.UnityLog("Error: Disconnected from server");
            connectionStatus = ConnectionStatus.Disconnnected;
            serverIp = string.Empty;
            client = null;
        }

        // Only position, animation, & directions updates are queued because they are sent in update
        public void SendQueue()
        {
            if (queuedMessages.Count == 0 || connectionStatus == ConnectionStatus.Disconnnected)
                return;

            Send(queuedMessages.ToArray());
            queuedMessages.Clear();
        }

        // Calculate the message and either immediately send it or queue it until the end of the frame
        private void Send(byte[] data, byte dataType, bool queueMessage)
        {
            if (data == null || data.Length == 0 || connectionStatus == ConnectionStatus.Disconnnected)
                return;

            List<byte> bytes = new List<byte>(BitConverter.GetBytes((ushort)data.Length));
            bytes.Add(dataType);
            bytes.AddRange(data);

            if (queueMessage)
            {
                queuedMessages.AddRange(bytes.ToArray());
            }
            else
            {
                Send(bytes.ToArray());
            }
        }

        // Actually send the byte array to the server
        private void Send(byte[] data)
        {
            try
            {
                //Main.UnityLog($"Sending {data.length} bytes");
                client.Write(data);
            }
            catch (System.IO.IOException)
            {
                Disconnect();
                Main.Multiplayer.onDisconnect();
            }
        }

        // Data should be formatted as length length type data
        private void Receive(object sender, DataReceivedEventArgs message)
        {
            //Main.UnityLog("Bytes received: " + message.data.Length);

            int startIdx = 0;
            while (startIdx < message.data.Length - 3)
            {
                ushort length = BitConverter.ToUInt16(message.data, startIdx);
                byte type = message.data[startIdx + 2];
                byte[] messageData = new byte[length];
                for (int i = 0; i < messageData.Length; i++)
                    messageData[i] = message.data[startIdx + 3 + i];

                processDataReceived(type, messageData);
                startIdx += 3 + length;
            }
            if (startIdx != message.data.Length)
                Main.UnityLog("Received data was formatted incorrectly");

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
                    case 7:
                        receivePlayerConnection(data); break;
                    case 8:
                        receivePlayerProgress(data); break;
                    default:
                        Main.UnityLog($"Data type '{type}' is not valid"); break;
                }
            }
        }

        // Retrieves the player name from the beginning of data & returns the new start idx
        private int getPlayerNameFromData(byte[] data, out string name)
        {
            byte nameLength = data[0];
            name = Encoding.UTF8.GetString(data, 1, nameLength);
            return nameLength + 1;
        }

        #region Send functions

        // Send this player's updated position
        public void sendPlayerPostition(float xPos, float yPos)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(xPos));
            bytes.AddRange(BitConverter.GetBytes(yPos));
            Send(bytes.ToArray(), 0, true);
        }

        // Send this player's updated animation
        public void sendPlayerAnimation(byte animation)
        {
            Send(new byte[] { animation }, 1, true);
        }

        // Send that this player entered a scene
        public void sendPlayerEnterScene(string scene)
        {
            Send(Encoding.UTF8.GetBytes(scene), 2, false);
        }

        // Send that this player left a scene
        public void sendPlayerLeaveScene()
        {
            Send(new byte[] { 0 }, 3, false);
        }

        // Send this player's updated direction
        public void sendPlayerDirection(bool direction)
        {
            Send(BitConverter.GetBytes(direction), 4, true);
        }

        // Send this player's updated skin
        public void sendPlayerSkin(string skin)
        {
            Send(Encoding.UTF8.GetBytes(skin), 5, false);
        }

        // Send this player's introductory data
        public void sendPlayerIntro(string name)
        {
            Send(Encoding.UTF8.GetBytes(name), 6, false);
        }

        // Send a new item/flag/stat/etc...
        public void sendPlayerProgress(byte type, byte value, string id)
        {
            List<byte> bytes = new List<byte>();
            bytes.Add(type);
            bytes.Add(value);
            bytes.AddRange(Encoding.UTF8.GetBytes(id));
            Send(bytes.ToArray(), 8, false);
        }

        #endregion Send functions

        #region Receive functions

        // Received a player's updated position
        private void receivePlayerPostition(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            float xPos = BitConverter.ToSingle(data, startIdx);
            float yPos = BitConverter.ToSingle(data, startIdx + 4);

            // Update specified player with new data
            Main.Multiplayer.playerPositionUpdated(playerName, xPos, yPos);
        }

        // Recieved a player's updated animation
        private void receivePlayerAnimation(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            byte animation = data[startIdx];

            // Update specified player with new data
            Main.Multiplayer.playerAnimationUpdated(playerName, animation);
        }

        // Received that a player entered a scene
        private void receivePlayerEnterScene(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            string scene = Encoding.UTF8.GetString(data, startIdx, data.Length - startIdx);

            // Create the new player object
            Main.Multiplayer.playerEnteredScene(playerName, scene);
        }

        // Received that a player left a scene
        private void receivePlayerLeaveScene(byte[] data)
        {
            // Remove the player object
            Main.Multiplayer.playerLeftScene(Encoding.UTF8.GetString(data));
        }

        // Received a player's updated direction
        private void receivePlayerDirection(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            bool direction = BitConverter.ToBoolean(data, startIdx);

            // Update specified player with new data
            Main.Multiplayer.playerDirectionUpdated(playerName, direction);
        }

        // Received a player's updated skin
        private void receivePlayerSkin(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            string skin = Encoding.UTF8.GetString(data, startIdx, data.Length - startIdx);

            // Update specified player with new data
            Main.Multiplayer.playerSkinUpdated(playerName, skin);
        }

        // Received their intro response
        private void receivePlayerIntro(byte[] data)
        {
            byte response = data[0];

            if (response == 0)
            {
                // Successfully connected and can sync data now
                connectionStatus = ConnectionStatus.Connected;
            }
            else
            {
                // Rejected from server
                connectionStatus = ConnectionStatus.Disconnnected;
                client.Disconnect();
                client = null;
            }

            Main.Multiplayer.playerIntroReceived(response);
        }

        // Received that a player connected or disconnected
        private void receivePlayerConnection(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            bool connected = BitConverter.ToBoolean(data, startIdx);

            // Update player list with new/removed player
            Main.Multiplayer.playerConnectionReceived(playerName, connected);
        }

        // Received an item/flag/stat/etc..
        private void receivePlayerProgress(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            byte type = data[startIdx];
            byte value = data[startIdx + 1];
            string id = Encoding.UTF8.GetString(data, startIdx + 2, data.Length - startIdx - 2);

            // Give new progress update
            Main.Multiplayer.playerProgressReceived(playerName, id, type, value);
        }

        #endregion Receive functions
    }
}
