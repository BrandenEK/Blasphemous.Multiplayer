using System;
using System.Collections.Generic;
using System.Text;
using SimpleTCP;

namespace BlasClient
{
    public class Client
    {
        public bool connected { get; private set; }

        private string ipAddress;
        private SimpleTcpClient client;

        public Client(string ip)
        {
            ipAddress = ip;
        }

        public bool Connect()
        {
            try
            {
                client = new SimpleTcpClient();
                client.Connect(ipAddress, 25565);
                //client.Events.Connected += clientConnected;
                //client.Events.Disconnected += clientDisconnected;
                client.DataReceived += Receive;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }
            //catch (TimeoutException)
            //{
            //    Output.error($"Client timed out attempting to connect to the server at {IpAddress}:{port}");
            //    Program.EndProgram();
            //}

            connected = true;
            return true;
        }

        private void Send(byte[] data, byte dataType)
        {
            if (connected && data != null && data.Length > 0)
            {
                List<byte> list = new List<byte>(BitConverter.GetBytes((ushort)data.Length));
                list.Add(dataType);
                list.AddRange(data);
                Console.WriteLine($"Sending {list.Count} bytes");
                client.Write(list.ToArray());
            }
        }

        // Data should be formatted as length length type data
        private void Receive(object sender, DataReceivedEventArgs message)
        {
            Console.WriteLine("Bytes received: " + message.data.Length);

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
                Console.WriteLine("Received data was formatted incorrectly");

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
                    default:
                        Console.WriteLine($"Data type '{type}' is not valid"); break;
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
        public void sendPlayerPostition(float xPos, float yPos, bool facingDirection)
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(xPos));
            bytes.AddRange(BitConverter.GetBytes(yPos));
            bytes.AddRange(BitConverter.GetBytes(facingDirection));

            Send(bytes.ToArray(), 0);
        }

        // Send this player's updated animation
        public void sendPlayerAnimation(string animation)
        {
            Send(Encoding.UTF8.GetBytes(animation), 1);
        }

        // Send that this player entered a scene
        public void sendPlayerEnterScene(string scene)
        {
            Send(Encoding.UTF8.GetBytes(scene), 2);
        }

        // Send that this player left a scene
        public void sendPlayerLeaveScene()
        {
            Send(new byte[] { 0 }, 3);
        }

        public void sendPlayerName(string name) // old
        {
            Send(Encoding.UTF8.GetBytes(name), 0);
        }

        public void sendPlayerUpdate(PlayerStatus status) // old
        {
            Send(status.loadStatus(), 1);
        }

        #endregion Send functions

        #region Receive functions

        // Received a player's updated position
        public void receivePlayerPostition(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            float xPos = BitConverter.ToSingle(data, startIdx);
            float yPos = BitConverter.ToSingle(data, startIdx + 4);
            bool facingDirection = BitConverter.ToBoolean(data, startIdx + 8);

            // Update specified player with new data
            Main.Multiplayer.playerPositionUpdated(playerName, xPos, yPos, facingDirection);
        }

        // Recieved a player's updated animation
        public void receivePlayerAnimation(byte[] data)
        {
            int startIdx = getPlayerNameFromData(data, out string playerName);
            string animation = Encoding.UTF8.GetString(data, startIdx, data.Length - startIdx);

            // Update specified player with new data
            Main.Multiplayer.playerAnimationUpdated(playerName, animation);
        }

        // Received that a player entered a scene
        public void receivePlayerEnterScene(byte[] data)
        {
            // Create the new player object
            Main.Multiplayer.playerEnteredScene(Encoding.UTF8.GetString(data));
        }

        // Received that a player left a scene
        public void receivePlayerLeaveScene(byte[] data)
        {
            // Remove the player object
            Main.Multiplayer.playerLeftScene(Encoding.UTF8.GetString(data));
        }

        private void receivePlayerUpdate(byte[] data) // old
        {
            List<PlayerStatus> players = new List<PlayerStatus>();
            int startIdx = 0;
            while (startIdx < data.Length)
            {
                PlayerStatus status = new PlayerStatus();
                startIdx = status.updateStatus(data, startIdx);
                players.Add(status);
            }
            Main.Multiplayer.updatePlayers(players);
        }

        #endregion Receive functions


        //private void clientConnected(object sender, ClientConnectedEventArgs e)
        //{
        //    Console.WriteLine("Successfully connected to the server at " + e.IpPort);
        //}

        //private void clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        //{
        //    Console.WriteLine("Disconnected from the server at " + e.IpPort);
        //}
    }
}
