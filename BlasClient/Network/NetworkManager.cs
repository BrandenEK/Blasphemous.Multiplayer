using BlasClient.ProgressSync;
using BlasClient.PvP;
using Framework.Managers;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BlasClient.Network
{
    public class NetworkManager
    {
        // For now the receive functions are public, make them private once client is implemented
        // Check for connection is all of these

        private ConnectionStatus _connectionStatus = ConnectionStatus.Disconnected;
        private SimpleTcpClient _client;
        private string _serverIp = string.Empty;

        public bool IsConnected => _connectionStatus == ConnectionStatus.Connected && _client != null;
        public string ServerIP => _serverIp;

        private readonly List<byte> messageQueue = new ();

        // Connection

        public bool Connect(string ipAddress, string playerName, string password)
        {
            if (_connectionStatus != ConnectionStatus.Disconnected) return false;

            try
            {
                _client = new SimpleTcpClient();
                _client.Connect(ipAddress, Main.Multiplayer.config.serverPort);
                _client.DataReceived += Receive;
                _client.TcpClient.NoDelay = true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }

            OnConnect(ipAddress, playerName, password);
            return true;
        }

        public void Disconnect()
        {
            _client.Disconnect();
            OnDisconnect();
        }

        private void OnConnect(string ipAddress, string playerName, string password)
        {
            _connectionStatus = ConnectionStatus.Attempting;
            _serverIp = ipAddress;
            SendIntro(playerName, password);

            Main.Multiplayer.Log("Connected to server: " + ipAddress);
            Main.Multiplayer.OnConnect(ipAddress, playerName, password);
        }

        private void OnDisconnect()
        {
            _connectionStatus = ConnectionStatus.Disconnected;
            _serverIp = string.Empty;
            _client = null;

            Main.Multiplayer.Log("Disconnected from server");
            Main.Multiplayer.OnDisconnect();
        }

        // Server

        private void QueueMesssage(byte[] data, NetworkType type)
        {
            if (data == null || data.Length == 0 || _connectionStatus == ConnectionStatus.Disconnected) return;

            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes((ushort)data.Length));
            bytes.Add((byte)type);
            bytes.AddRange(data);

            messageQueue.AddRange(bytes);
        }

        public void SendQueue()
        {
            if (messageQueue.Count == 0 || _connectionStatus == ConnectionStatus.Disconnected) return;

            try
            {
                _client.Write(messageQueue.ToArray());
            }
            catch (System.IO.IOException)
            {
                OnDisconnect();
            }
            finally
            {
                messageQueue.Clear();
            }
        }

        // Data should be formatted as length length type data
        private void Receive(object sender, DataReceivedEventArgs message)
        {
            int startIdx = 0;
            while (startIdx < message.data.Length - 3)
            {
                ushort length = BitConverter.ToUInt16(message.data, startIdx);
                byte type = message.data[startIdx + 2];
                byte[] messageData = new byte[length];
                for (int i = 0; i < messageData.Length; i++)
                    messageData[i] = message.data[startIdx + 3 + i];

                switch ((NetworkType)type)
                {
                    case NetworkType.Position:      ReceivePosition(messageData); break;
                    case NetworkType.Animation:     ReceiveAnimation(messageData); break;
                    case NetworkType.Direction:     ReceiveDirection(messageData); break;
                    case NetworkType.EnterScene:    ReceiveEnterScene(messageData); break;
                    case NetworkType.LeaveScene:    ReceiveLeaveScene(messageData); break;
                    case NetworkType.Skin:          ReceiveSkin(messageData); break;
                    case NetworkType.Team:          ReceiveTeam(messageData); break;
                    case NetworkType.Connection:    ReceiveConnection(messageData); break;
                    case NetworkType.Intro:         ReceiveIntro(messageData); break;
                    case NetworkType.Progress:      ReceiveProgress(messageData); break;
                    case NetworkType.Attack:        ReceiveAttack(messageData); break;
                    case NetworkType.Effect:        ReceiveEffect(messageData); break;

                }
                startIdx += 3 + length;
            }
            if (startIdx != message.data.Length)
                Main.Multiplayer.Log("Received data was formatted incorrectly");
        }

        private int ExtractNameFromData(byte[] data, out string name)
        {
            byte nameLength = data[0];
            name = Encoding.UTF8.GetString(data, 1, nameLength);
            return nameLength + 1;
        }

        // Position

        public void SendPosition(Vector2 position)
        {
            List<byte> bytes = new();
            bytes.AddRange(BitConverter.GetBytes(position.x));
            bytes.AddRange(BitConverter.GetBytes(position.y));
            QueueMesssage(bytes.ToArray(), NetworkType.Position);
        }

        private void ReceivePosition(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            float xpos = BitConverter.ToSingle(data, startIdx);
            float ypos = BitConverter.ToSingle(data, startIdx + 4);

            Main.Multiplayer.PlayerManager.ReceivePosition(playerName, new Vector2(xpos, ypos));
        }
        
        // Animation

        public void SendAnimation(byte animation)
        {
            QueueMesssage(new byte[] { animation }, NetworkType.Animation);
        }

        private void ReceiveAnimation(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            byte animation = data[startIdx];

            Main.Multiplayer.PlayerManager.ReceiveAnimation(playerName, animation);
        }

        // Direction

        public void SendDirection(bool direction)
        {
            QueueMesssage(BitConverter.GetBytes(direction), NetworkType.Direction);
        }

        private void ReceiveDirection(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            bool directon = BitConverter.ToBoolean(data, startIdx);

            Main.Multiplayer.PlayerManager.ReceiveDirection(playerName, directon);
        }

        // Enter scene

        public void SendEnterScene(string sceneName)
        {
            QueueMesssage(Encoding.UTF8.GetBytes(sceneName), NetworkType.EnterScene);
        }

        private void ReceiveEnterScene(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            string scene = Encoding.UTF8.GetString(data, startIdx, data.Length - startIdx);

            // TODO
            Main.Multiplayer.playerList.setPlayerScene(playerName, scene);
            if (Core.LevelManager.currentLevel.LevelName == scene)
                Main.Multiplayer.PlayerManager.queuePlayer(playerName, true);
            Main.Multiplayer.MapManager.QueueMapUpdate();
        }

        // Leave scene

        public void SendLeaveScene()
        {
            QueueMesssage(new byte[] { 0 }, NetworkType.LeaveScene);
        }

        private void ReceiveLeaveScene(byte[] data)
        {
            ExtractNameFromData(data, out string playerName);

            // TODO
            if (Core.LevelManager.currentLevel.LevelName == Main.Multiplayer.playerList.getPlayerScene(playerName))
                Main.Multiplayer.PlayerManager.queuePlayer(playerName, false);
            Main.Multiplayer.playerList.setPlayerScene(playerName, "");
            Main.Multiplayer.MapManager.QueueMapUpdate();
        }

        // Skin

        public void SendSkin(string skinName)
        {
            bool originalSkin = false;
            foreach (string ogskin in originalSkins)
            {
                if (skinName == ogskin)
                {
                    originalSkin = true;
                    break;
                }
            }

            byte[] skinData = originalSkin ? Encoding.UTF8.GetBytes(skinName) : Core.ColorPaletteManager.GetColorPaletteById(skinName).texture.EncodeToPNG();
            QueueMesssage(skinData, NetworkType.Skin);
        }

        private void ReceiveSkin(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            byte[] skinData = new byte[data.Length - startIdx];
            for (int i = 0; i < skinData.Length; i++)
            {
                skinData[i] = data[i + startIdx];
            }

            Main.Multiplayer.UpdateSkinData(playerName, skinData);
        }

        // Team

        public void SendTeam(byte team)
        {
            QueueMesssage(new byte[] { team }, NetworkType.Team);
        }

        private void ReceiveTeam(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            byte team = data[startIdx];

            Main.Multiplayer.playerList.setPlayerTeam(playerName, team);
            if (Main.Multiplayer.CurrentlyInLevel)
                Main.Multiplayer.RefreshPlayerColors();
        }

        // Connection

        public void SendConnection()
        {
            // A client will never send a player connection, this is only sent by the server
        }

        private void ReceiveConnection(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            bool connected = BitConverter.ToBoolean(data, startIdx);

            if (connected)
            {
                // Add this player to the list of connected players
                Main.Multiplayer.playerList.AddPlayer(playerName);
            }
            else
            {
                // Remove this player from the list of connected players
                // playerLeftScene(playerName); // Need to actually remove the player object if same scene
                Main.Multiplayer.playerList.RemovePlayer(playerName);
            }
            Main.Multiplayer.NotificationManager.DisplayNotification($"{playerName} {Main.Multiplayer.Localize(connected ? "join" : "leave")}");
        }

        // Intro

        public void SendIntro(string playerName, string password)
        {
            List<byte> bytes = new ();
            if (password == null)
            {
                bytes.Add(0);
            }
            else
            {
                bytes.Add((byte)password.Length);
                bytes.AddRange(Encoding.UTF8.GetBytes(password));
            }
            bytes.AddRange(Encoding.UTF8.GetBytes(playerName));

            QueueMesssage(bytes.ToArray(), NetworkType.Intro);
        }

        private void ReceiveIntro(byte[] data)
        {
            byte response = data[0];

            if (response == 0)
            {
                // Successfully connected and can sync data now
                _connectionStatus = ConnectionStatus.Connected;
            }
            else
            {
                // Rejected from server
                Disconnect();
            }

            // Call intro receive
            Main.Multiplayer.ProcessIntroResponse(response);
        }

        // Progress

        public void SendProgress(ProgressUpdate progress)
        {
            List<byte> bytes = new ();
            bytes.Add((byte)progress.Type);
            bytes.Add(progress.Value);
            bytes.AddRange(Encoding.UTF8.GetBytes(progress.Id));

            QueueMesssage(bytes.ToArray(), NetworkType.Progress);
        }

        private void ReceiveProgress(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            ProgressType progressType = (ProgressType)data[startIdx];
            byte progressValue = data[startIdx + 1];
            string progressId = Encoding.UTF8.GetString(data, startIdx + 2, data.Length - startIdx - 2);

            ProgressUpdate progress = new ProgressUpdate(progressId, progressType, progressValue);
            Main.Multiplayer.ProgressManager.ReceiveProgress(progress);
            Main.Multiplayer.ProcessRecievedStat(progress);

            if (playerName != "*")
                Main.Multiplayer.NotificationManager.DisplayProgressNotification(playerName, progress);
        }

        // Attack

        public void SendAttack(string hitPlayerName, AttackType attackType)
        {
            List<byte> bytes = new ();
            bytes.Add((byte)attackType);
            bytes.AddRange(Encoding.UTF8.GetBytes(hitPlayerName));

            QueueMesssage(bytes.ToArray(), NetworkType.Attack);
        }

        private void ReceiveAttack(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string attackerName);
            AttackType attackType = (AttackType)data[startIdx];
            string receiverName = Encoding.UTF8.GetString(data, startIdx + 1, data.Length - startIdx - 1);

            Main.Multiplayer.AttackManager.ReceiveAttack(attackerName, receiverName, attackType);
        }

        // Effect

        public void SendEffect(EffectType effectType)
        {
            QueueMesssage(new byte[] { (byte)effectType }, NetworkType.Effect);
        }

        private void ReceiveEffect(byte[] data)
        {
            int startIdx = ExtractNameFromData(data, out string playerName);
            EffectType effectType = (EffectType)data[startIdx];

            Main.Multiplayer.AttackManager.ReceiveEffect(playerName, effectType);
        }

        // Temp data

        private string[] originalSkins = new string[]
        {
            "PENITENT_DEFAULT",
            "PENITENT_ENDING_A",
            "PENITENT_ENDING_B",
            "PENITENT_OSSUARY",
            "PENITENT_BACKER",
            "PENITENT_DELUXE",
            "PENITENT_ALMS",
            "PENITENT_PE01",
            "PENITENT_PE02",
            "PENITENT_PE03",
            "PENITENT_BOSSRUSH",
            "PENITENT_DEMAKE",
            "PENITENT_ENDING_C",
            "PENITENT_SIERPES",
            "PENITENT_ISIDORA",
            "PENITENT_BOSSRUSH_S",
            "PENITENT_GAMEBOY",
            "PENITENT_KONAMI"
        };
    }
}
