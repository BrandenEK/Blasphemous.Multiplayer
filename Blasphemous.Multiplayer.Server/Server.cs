using System;
using System.Collections.Generic;
using System.Text;
using SimpleTCP;

namespace Blasphemous.Multiplayer.Server
{
    public class Server
    {
        private SimpleTcpServer server;

        private Dictionary<string, PlayerStatus> connectedPlayers;

        public Dictionary<string, PlayerStatus> getPlayers()
        {
            return connectedPlayers;
        }

        public bool DelayDisabled
        {
            get => server != null && server.DelayDisabled;
            set
            {
                if (server != null)
                    server.DelayDisabled = value;
            }
        }

        public bool Start()
        {
            try
            {
                server = new SimpleTcpServer();
                server.ClientConnected += clientConnected;
                server.ClientDisconnected += clientDisconnected;
                server.DataReceived += Receive;
                server.Start(Core.config.serverPort);
                server.DelayDisabled = true;
            }
            catch (System.Net.Sockets.SocketException)
            {
                return false;
            }

            connectedPlayers = new Dictionary<string, PlayerStatus>();
            return true;
        }

        private void Send(string ip, byte[] data, NetworkType dataType)
        {
            if (data != null && data.Length > 0)
            {
                List<byte> list = new List<byte>(BitConverter.GetBytes((ushort)data.Length));
                list.Add((byte)dataType);
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
                NetworkType type = (NetworkType)e.data[startIdx + 2];
                byte[] messageData = e.data[(startIdx + 3)..(startIdx + 3 + length)];

                processDataReceived(type, messageData);
                startIdx += 3 + length;
            }
            if (startIdx != e.data.Length)
                Core.displayError("Received data was formatted incorrectly");

            // Determines which received function to call based on the type
            void processDataReceived(NetworkType type, byte[] data)
            {
                switch (type)
                {
                    case NetworkType.Position:      ReceivePosition(e.ip, data); break;
                    case NetworkType.Animation:     ReceiveAnimation(e.ip, data); break;
                    case NetworkType.Direction:     ReceiveDirection(e.ip, data); break;
                    case NetworkType.EnterScene:    receivePlayerEnterScene(e.ip, data); break;
                    case NetworkType.LeaveScene:    receivePlayerLeaveScene(e.ip, data); break;
                    case NetworkType.Skin:          receivePlayerSkin(e.ip, data); break;
                    case NetworkType.Team:          receivePlayerTeam(e.ip, data); break;
                    case NetworkType.Intro:         receivePlayerIntro(e.ip, data); break;
                    case NetworkType.Progress:      receivePlayerProgress(e.ip, data); break;
                    case NetworkType.Attack:        receivePlayerAttack(e.ip, data); break;
                    case NetworkType.Effect:        receivePlayerEffect(e.ip, data); break;
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

            // Send that this player has disconnected & remove them
            sendPlayerConnection(e.ip, false);
            connectedPlayers.Remove(e.ip);
            Core.removeUnusedGameData(connectedPlayers);
        }

        private PlayerStatus getCurrentPlayer(string ip)
        {
            if (connectedPlayers.ContainsKey(ip))
                return connectedPlayers[ip];

            Core.displayError("Data for " + ip + " has not been created yet!");
            return new PlayerStatus("");
        }

        private List<byte> addPlayerNameToData(string name)
        {
            byte[] nameBytes = Encoding.UTF8.GetBytes(name);

            List<byte> data = new List<byte>();
            data.Add((byte)nameBytes.Length);
            data.AddRange(nameBytes);
            return data;
        }

        // Position

        private void SendPosition(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated position
                    Send(ip, GetPositionPacket(current), NetworkType.Position);
                }
            }
        }

        private void ReceivePosition(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.xPos = BitConverter.ToSingle(data, 0);
            current.yPos = BitConverter.ToSingle(data, 4);

            SendPosition(playerIp);
        }

        private byte[] GetPositionPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(BitConverter.GetBytes(player.xPos));
            bytes.AddRange(BitConverter.GetBytes(player.yPos));
            return bytes.ToArray();
        }

        // Animation

        private void SendAnimation(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated animation
                    Send(ip, GetAnimationPacket(current), NetworkType.Animation);
                }
            }
        }

        private void ReceiveAnimation(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.animation = data[0];

            SendAnimation(playerIp);
        }

        private byte[] GetAnimationPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.Add(player.animation);
            return bytes.ToArray();
        }

        // Direction

        private void SendDirection(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's updated direction
                    Send(ip, GetDirectionPacket(current), NetworkType.Direction);
                }
            }
        }

        private void ReceiveDirection(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.facingDirection = BitConverter.ToBoolean(data);

            SendDirection(playerIp);
        }

        private byte[] GetDirectionPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(BitConverter.GetBytes(player.facingDirection));
            return bytes.ToArray();
        }

        // Enter scene

        private void sendPlayerEnterScene(string playerIp) // Optimize these send functions to combine them together
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    // Send that this player has entered a new scene
                    Send(ip, getScenePacket(current), NetworkType.EnterScene);

                    if (current.isInSameScene(connectedPlayers[ip]))
                    {
                        // If in same scene, also send position data to each one
                        Send(ip, GetPositionPacket(current), NetworkType.Position);
                        Send(ip, GetAnimationPacket(current), NetworkType.Animation);
                        Send(ip, GetDirectionPacket(current), NetworkType.Direction);

                        Send(playerIp, GetPositionPacket(connectedPlayers[ip]), NetworkType.Position);
                        Send(playerIp, GetAnimationPacket(connectedPlayers[ip]), NetworkType.Animation);
                        Send(playerIp, GetDirectionPacket(connectedPlayers[ip]), NetworkType.Direction);
                    }
                }
            }
        }

        private void receivePlayerEnterScene(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.sceneName = Encoding.UTF8.GetString(data);

            sendPlayerEnterScene(playerIp);
        }

        private byte[] getScenePacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(Encoding.UTF8.GetBytes(player.sceneName));
            return bytes.ToArray();
        }

        // Leave scene

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
                    Send(ip, Encoding.UTF8.GetBytes(current.name), NetworkType.LeaveScene);
                }
            }
        }

        private void receivePlayerLeaveScene(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);

            sendPlayerLeaveScene(playerIp);
            current.sceneName = "";
        }

        // Skin

        private void sendPlayerSkin(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    // Send this player's updated skin
                    Send(ip, getSkinPacket(current), NetworkType.Skin);
                }
            }
        }

        private void receivePlayerSkin(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.skin = data;

            sendPlayerSkin(playerIp);
        }

        private byte[] getSkinPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(player.skin);
            return bytes.ToArray();
        }

        // Team

        private void sendPlayerTeam(string playerIp)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    Send(ip, getTeamPacket(current), NetworkType.Team);
                }
            }

            // Send all of the server data to this player for them to merge
            Core.displayMessage("Sending all server data to " + playerIp);
            for (byte i = 0; i < GameData.NUMBER_OF_PROGRESS_TYPES; i++)
            {
                Dictionary<string, byte> progressSet = Core.getTeamData(current.team).GetTeamProgressSet(i);
                foreach (string id in progressSet.Keys)
                {
                    byte value = progressSet[id];
                    if (i == 6 && id == "FLASK")
                    {
                        Core.displayMessage("Send all server flask: " + value);
                        value -= Core.getTeamData(current.team).GetTeamProgressValue(6, "FLASKHEALTH");
                        Core.displayMessage("Send all send flask: " + value);
                    }
                    Send(playerIp, getProgressPacket("*", i, value, id), NetworkType.Progress);
                }
            }
        }

        private void receivePlayerTeam(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            current.team = data[0];

            sendPlayerTeam(playerIp);
            Core.removeUnusedGameData(connectedPlayers);
        }

        private byte[] getTeamPacket(PlayerStatus player)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.Add(player.team);
            return bytes.ToArray();
        }

        // Connection

        private void sendPlayerConnection(string playerIp, bool connected)
        {
            // Send message to all other connected ips
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    Send(ip, getConnectionPacket(current, connected), NetworkType.Connection);
                }
            }
        }

        private byte[] getConnectionPacket(PlayerStatus player, bool connected)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(BitConverter.GetBytes(connected));
            return bytes.ToArray();
        }

        // Intro

        private void sendPlayerIntro(string playerIp, byte response)
        {
            Send(playerIp, getIntroPacket(response), NetworkType.Intro);

            // Only send rest of data if successful connection
            if (response > 0)
                return;

            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip)
                {
                    // Send all other connected players and their important data
                    Send(playerIp, getConnectionPacket(connectedPlayers[ip], true), NetworkType.Connection);
                    Send(playerIp, getSkinPacket(connectedPlayers[ip]), NetworkType.Skin);
                    Send(playerIp, getTeamPacket(connectedPlayers[ip]), NetworkType.Team);
                    if (connectedPlayers[ip].sceneName != "")
                    {
                        Send(playerIp, getScenePacket(connectedPlayers[ip]), NetworkType.EnterScene);
                        if (current.isInSameScene(connectedPlayers[ip]))
                        {
                            Send(playerIp, GetPositionPacket(connectedPlayers[ip]), NetworkType.Position);
                            Send(playerIp, GetAnimationPacket(connectedPlayers[ip]), NetworkType.Animation);
                            Send(playerIp, GetDirectionPacket(connectedPlayers[ip]), NetworkType.Direction);
                        }
                    }
                }
            }
        }

        private void receivePlayerIntro(string playerIp, byte[] data)
        {
            // Load player name and password from data
            byte passwordLength = data[0], nameStartIdx = 1;
            string playerPassword = null;
            if (passwordLength > 0)
            {
                playerPassword = Encoding.UTF8.GetString(data, 1, passwordLength);
                nameStartIdx = (byte)(passwordLength + 1);
            }
            string playerName = Encoding.UTF8.GetString(data, nameStartIdx, data.Length - passwordLength - 1);

            // Ensure the password is correct
            string serverPassword = Core.config.password;
            if (serverPassword != null && serverPassword != "" && (playerPassword == null || playerPassword != serverPassword))
            {
                Core.displayMessage("Player connection rejected: Incorrect password");
                sendPlayerIntro(playerIp, 1);
                return;
            }

            // Ensure this ip address is not banned

            // Ensure the server doesn't have max number of players
            if (connectedPlayers.Count >= Core.config.maxPlayers)
            {
                Core.displayMessage("Player connection rejected: Player limit reached");
                sendPlayerIntro(playerIp, 3);
                return;
            }

            // Ensure there are no duplicate ips
            if (connectedPlayers.ContainsKey(playerIp))
            {
                Core.displayMessage("Player connection rejected: Duplicate ip address");
                sendPlayerIntro(playerIp, 4);
                return;
            }

            // Ensure there are no duplicate names
            foreach (PlayerStatus player in connectedPlayers.Values)
            {
                if (player.name == playerName)
                {
                    Core.displayMessage("Player connection rejected: Duplicate name");
                    sendPlayerIntro(playerIp, 5);
                    return;
                }
            }

            // Add new connected player
            Core.displayMessage("Player connection accepted");
            PlayerStatus newPlayer = new PlayerStatus(playerName);
            connectedPlayers.Add(playerIp, newPlayer);
            sendPlayerConnection(playerIp, true);
            sendPlayerIntro(playerIp, 0);
        }

        private byte[] getIntroPacket(byte response)
        {
            return new byte[] { response };
        }

        // Progress

        private void sendPlayerProgress(string playerIp, byte type, byte value, string id)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.team == getCurrentPlayer(ip).team)
                {
                    Send(ip, getProgressPacket(current.name, type, value, id), NetworkType.Progress);
                }
            }
        }

        private void receivePlayerProgress(string playerIp, byte[] data)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            byte progressType = data[0];
            byte progressValue = data[1];
            string progressId = Encoding.UTF8.GetString(data, 2, data.Length - 2);

            // Add the progress to the server data, and if it's new send it to the rest of the players
            if (!Core.getTeamData(current.team).AddTeamProgress(progressId, progressType, progressValue))
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
            else if (progressType == 12)
            {
                // Church donation
                Core.displayCustom($"Received new tear donation from {current.name}: {progressValue}", ConsoleColor.Green);
            }
            else if (progressType == 13)
            {
                // Miriam status
                Core.displayCustom($"Received new miriam status from {current.name}: {progressId}", ConsoleColor.Green);
            }

            // If this is a stat upgrade, might have to do something extra with flask/flaskhealth
            if (progressType == 6)
            {
                if (progressId == "FLASK")
                {
                    Core.displayMessage("Received flask level: " + progressValue);
                    byte flaskHealthUpgrades = Core.getTeamData(current.team).GetTeamProgressValue(6, "FLASKHEALTH");
                    progressValue -= flaskHealthUpgrades;
                    Core.displayMessage("Flask level sent out: " + progressValue);
                }
                else if (progressId == "FLASKHEALTH")
                {
                    byte flaskUpgrades = Core.getTeamData(current.team).GetTeamProgressValue(6, "FLASK");
                    Core.displayMessage("Flask level stored: " + flaskUpgrades);
                    Core.displayMessage("Flask level sent: " + (byte)(flaskUpgrades - progressValue));
                    sendPlayerProgress(playerIp, 6, (byte)(flaskUpgrades - progressValue), "FLASK");
                }
            }

            sendPlayerProgress(playerIp, progressType, progressValue, progressId);
        }

        private byte[] getProgressPacket(string nameOrStar, byte type, byte value, string id)
        {
            List<byte> bytes = addPlayerNameToData(nameOrStar);
            bytes.Add(type);
            bytes.Add(value);
            bytes.AddRange(Encoding.UTF8.GetBytes(id));
            return bytes.ToArray();
        }

        // Attack

        private void sendPlayerAttack(string playerIp, byte[] attackData)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's attack
                    Send(ip, getAttackPacket(current, attackData), NetworkType.Attack);
                }
            }
        }

        private void receivePlayerAttack(string playerIp, byte[] data)
        {
            sendPlayerAttack(playerIp, data);
        }

        private byte[] getAttackPacket(PlayerStatus player, byte[] attackData)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.AddRange(attackData);
            return bytes.ToArray();
        }

        // Effect

        private void sendPlayerEffect(string playerIp, byte effect)
        {
            PlayerStatus current = getCurrentPlayer(playerIp);
            foreach (string ip in connectedPlayers.Keys)
            {
                if (playerIp != ip && current.isInSameScene(connectedPlayers[ip]))
                {
                    // Send this player's attack
                    Send(ip, getEffectPacket(current, effect), NetworkType.Effect);
                }
            }
        }

        private void receivePlayerEffect(string playerIp, byte[] data)
        {
            sendPlayerEffect(playerIp, data[0]);
        }

        private byte[] getEffectPacket(PlayerStatus player, byte effect)
        {
            List<byte> bytes = addPlayerNameToData(player.name);
            bytes.Add(effect);
            return bytes.ToArray();
        }
    }
}
