using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using SimpleTcp;

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
                server = new SimpleTcpServer(ipAddress, 25565);
                server.Events.ClientConnected += clientConnected;
                server.Events.ClientDisconnected += clientDisconnected;
                server.Events.DataReceived += Receive;
                server.Start();
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }

            connectedPlayers = new Dictionary<string, PlayerStatus>();
            GameLoop();
            return true;
        }

        private async Task GameLoop()
        {
            while (true)
            {
                Core.displayMessage("Game loop tick");

                await Task.Delay(100);
            }
        }

        private void Send(string ip, byte[] data, byte dataType)
        {
            if (server.IsListening && data != null && data.Length > 0)
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
            Core.displayMessage("Bytes received: " + e.Data.Length);
            currentIp = e.IpPort;

            int startIdx = 0;
            while (startIdx < e.Data.Length - 3)
            {
                ushort length = BitConverter.ToUInt16(e.Data, startIdx);
                byte type = e.Data[startIdx + 2];
                byte[] messageData = e.Data[(startIdx + 3)..(startIdx + 3 + length)];

                processDataReceived(type, messageData);
                startIdx += 3 + length;
            }
            if (startIdx != e.Data.Length)
                Core.displayError("Received data was formatted incorrectly");
        }

        private void processDataReceived(byte type, byte[] data)
        {
            switch (type)
            {
                case 0:
                    receivePlayerName(data); break;
                case 1:
                    receivePlayerUpdate(data); break;
                default:
                    Core.displayError($"Data type '{type}' is not valid"); break;
            }
        }

        // Right after client connects, they send their name
        void receivePlayerName(byte[] data)
        {
            string name = Encoding.UTF8.GetString(data);
            connectedPlayers[currentIp].name = name;
            // Notification for join
        }

        // Every certain number of frames, a client will send data about position, orientation, sprite, etc.
        void receivePlayerUpdate(byte[] data)
        {
            string player = connectedPlayers[currentIp].name;
            Core.displayMessage("Updating player " + player);
        }

        private void clientConnected(object sender, ClientConnectedEventArgs e)
        {
            Core.displayMessage("Client connected at " + e.IpPort);
            connectedPlayers.Add(e.IpPort, new PlayerStatus());
        }

        private void clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Core.displayMessage("Client disconnected at " + e.IpPort);
            connectedPlayers.Remove(e.IpPort);
            // Noitification for leave
        }
    }
}
