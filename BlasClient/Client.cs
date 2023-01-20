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

        public void sendPlayerName(string name)
        {
            Send(Encoding.UTF8.GetBytes(name), 0);
        }

        public void sendPlayerUpdate(PlayerStatus status)
        {
            Send(status.loadStatus(), 1);
        }

        private void receivePlayerUpdate(byte[] data)
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
        }

        private void processDataReceived(byte type, byte[] data)
        {
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    receivePlayerUpdate(data); break;
                default:
                    Console.WriteLine($"Data type '{type}' is not valid"); break;
            }
        }


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
