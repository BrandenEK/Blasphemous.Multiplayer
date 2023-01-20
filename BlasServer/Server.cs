using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }

            connectedPlayers = new Dictionary<string, PlayerStatus>();
            return true;
        }

        private async Task GameLoop()
        {
            while (true)
            {
                //Core.displayMessage("Game loop tick");

                await Task.Delay(100);
            }
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

        void sendPlayerUpdate()
        {
            List<byte> allPlayers = new List<byte>();
            foreach (PlayerStatus status in connectedPlayers.Values)
                allPlayers.AddRange(status.loadStatus());
            Send(currentIp, allPlayers.ToArray(), 1);
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
            PlayerStatus status = new PlayerStatus();
            status.updateStatus(data);
            connectedPlayers[currentIp] = status;

            Core.displayMessage("Received status from " + status.name);
            Core.displayMessage(status.ToString());

            Core.displayMessage("Sending other player statuses");
            sendPlayerUpdate();
        }

        private void clientConnected(object sender, ClientConnectionEventArgs e)
        {
            Core.displayMessage("Client connected at " + e.ip);
            connectedPlayers.Add(e.ip, new PlayerStatus());
        }

        private void clientDisconnected(object sender, ClientConnectionEventArgs e)
        {
            Core.displayMessage("Client disconnected at " + e.ip);
            connectedPlayers.Remove(e.ip);
            // Noitification for leave
        }
    }
}
