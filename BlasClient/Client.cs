using System;
using System.Collections.Generic;
using System.Text;
using SimpleTcp;

namespace BlasClient
{
    public class Client
    {
        private string ipAddress;
        private string playerName;
        private SimpleTcpClient client;

        public Client(string ip, string name)
        {
            ipAddress = ip;
            playerName = name;
        }

        public bool Connect()
        {
            try
            {
                client = new SimpleTcpClient(ipAddress, 25565);
                client.Events.Connected += clientConnected;
                client.Events.Disconnected += clientDisconnected;
                client.Events.DataReceived += Receive;
                client.Connect();
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

            sendPlayerName();
            return true;
        }

        private void Send(byte[] data, byte dataType)
        {
            if (client.IsConnected && data != null && data.Length > 0)
            {
                List<byte> list = new List<byte>(BitConverter.GetBytes((ushort)data.Length));
                list.Add(dataType);
                list.AddRange(data);
                Console.WriteLine($"Sending {list.Count} bytes");
                client.Send(list.ToArray());
            }
        }

        public void sendPlayerName()
        {
            Send(Encoding.UTF8.GetBytes(playerName), 0);
        }

        public void sendPlayerUpdate()
        {
            Send(new byte[] { 1, 2, 3 }, 1);
        }

        // Data should be formatted as length length type data
        private void Receive(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Bytes received: " + e.Data.Length);

            int startIdx = 0;
            while (startIdx < e.Data.Length - 3)
            {
                ushort length = BitConverter.ToUInt16(e.Data, startIdx);
                byte type = e.Data[startIdx + 2];
                byte[] messageData = new byte[length];
                for (int i = 0; i < messageData.Length; i++)
                    messageData[i] = e.Data[startIdx + 3 + i];

                processDataReceived(type, messageData);
                startIdx += 3 + length;
            }
            if (startIdx != e.Data.Length)
                Console.WriteLine("Received data was formatted incorrectly");
        }

        private void processDataReceived(byte type, byte[] data)
        {
            switch (type)
            {
                case 0:
                    break;
                case 1:
                    break;
                default:
                    Console.WriteLine($"Data type '{type}' is not valid"); break;
            }
        }


        private void clientConnected(object sender, ClientConnectedEventArgs e)
        {
            Console.WriteLine("Successfully connected to the server at " + e.IpPort);
        }

        private void clientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("Disconnected from the server at " + e.IpPort);
        }
    }
}
