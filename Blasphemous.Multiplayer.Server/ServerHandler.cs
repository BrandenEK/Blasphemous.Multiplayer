using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Server;
using Blasphemous.Multiplayer.Common;
using Blasphemous.Multiplayer.Common.Enums;
using Blasphemous.Multiplayer.Common.Packets;
using Blasphemous.Multiplayer.Server.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Blasphemous.Multiplayer.Server;

public class ServerHandler
{
    // TODO: temporary data
    private readonly int _maxPlayers;
    private readonly string _password;

    private readonly NetworkServer _server;

    public ServerHandler(int maxPlayers, string password)
    {
        // Change how this works later
        _maxPlayers = maxPlayers;
        _password = password;

        _server = new NetworkServer(NetworkHelper.CreateSerializer());
        _server.OnClientConnected += OnClientConnected;
        _server.OnClientDisconnected += OnClientDisconnected;
        _server.OnPacketReceived += OnPacketReceived;
        _server.OnErrorReceived += OnErrorReceived;
        StartReadThread();
    }

    public bool Start(int port)
    {
        try
        {
            _server.Start(port);
        }
        catch (SocketException) // Maybe handle the network exception as well ??
        {
            return false;
        }

        _connectedPlayers.Clear(); // TODO: this was old as well
        return true;
    }

    private void OnClientConnected(string ip)
    {
        Logger.Info($"Client connected at {ip}");
    }

    private void OnClientDisconnected(string ip)
    {
        Logger.Info($"Client disconnected at {ip}"); // TODO: everything after this was old

        if (!TryGetPlayer(ip, out PlayerInfo current))
            return;

        // Send that this player has disconnected & remove them
        _connectedPlayers.Remove(ip);

        foreach (var player in _connectedPlayers.Values)
        {
            _server.Send(player.Ip, new QuitResponsePacket(current.Name));
        }

        //Core.removeUnusedGameData(_connectedPlayers);
    }

    private void OnErrorReceived(string ip, NetworkException exception)
    {
        Logger.Error($"Received error from {ip}: {exception}");
    }

    private void StartReadThread()
    {
        var thread = new Thread(ReadLoop);
        thread.IsBackground = true;
        thread.Start();
    }

    private void ReadLoop()
    {
        while (true)
        {
            if (_server.IsActive)
            {
                _server.Receive();
                _server.Flush();
            }

            Thread.Sleep(READ_INTERVAL_MS);
        }
    }

    private const int READ_INTERVAL_MS = 10;

    // =======================================
    // New (old) stuff that I want to refactor
    // =======================================

    private Dictionary<string, PlayerInfo> _connectedPlayers = [];

    public Dictionary<string, PlayerInfo> GetPlayers()
    {
        return _connectedPlayers;
    }

    // Helper methods

    private bool TryGetPlayer(string ip, out PlayerInfo player)
    {
        if (_connectedPlayers.TryGetValue(ip, out player))
            return true;

        Logger.Warn($"Player {ip} does not exist in the server");
        return false;
    }

    private bool InSameScene(PlayerInfo p1, PlayerInfo p2)
    {
        return !string.IsNullOrEmpty(p1.Scene) && p1.Scene == p2.Scene;
    }

    // Receiving packets

    private void OnPacketReceived(string ip, BasePacket packet)
    {
        Logger.Warn($"Received packet of type {packet.GetType().Name} from {ip}");

        switch (packet)
        {
            case PositionPacket position:
                ReceivePosition(ip, position);
                break;
            case AnimationPacket animation:
                ReceiveAnimation(ip, animation);
                break;
            case DirectionPacket direction:
                ReceiveDirection(ip, direction);
                break;
            case ScenePacket scene:
                ReceiveScene(ip, scene);
                break;
            case SkinPacket skin:
                ReceiveSkin(ip, skin);
                break;
            case IntroPacket intro:
                ReceiveIntro(ip, intro);
                break;
            case AttackPacket attack:
                ReceiveAttack(ip, attack);
                break;
            case EffectPacket effect:
                ReceiveEffect(ip, effect);
                break;
            case ProgressPacket progress:
                ReceiveProgress(ip, progress);
                break;
            case PingPacket ping:
                ReceivePing(ip, ping);
                break;
            default:
                Logger.Error($"Received unexpected packet from {ip}: {packet.GetType().Name}");
                break;
        }
    }

    private void ReceivePosition(string playerIp, PositionPacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Update player's stored position
        current.UpdatePosition(packet.X, packet.Y);

        // Send updated position
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip && InSameScene(current, x)))
        {
            _server.Send(player.Ip, new PositionResponsePacket(current.Name, current.XPosition, current.YPosition));
        }
    }

    private void ReceiveAnimation(string playerIp, AnimationPacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Update player's stored animation
        current.UpdateAnimation(packet.Animation);

        // Send updated animation
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip && InSameScene(current, x)))
        {
            _server.Send(player.Ip, new AnimationResponsePacket(current.Name, current.Animation));
        }
    }

    private void ReceiveDirection(string playerIp, DirectionPacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Update player's stored direction
        current.UpdateDirection(packet.Direction);

        // Send updated direction
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip && InSameScene(current, x)))
        {
            _server.Send(player.Ip, new DirectionResponsePacket(current.Name, current.Direction));
        }
    }

    private void ReceiveScene(string playerIp, ScenePacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        if (string.IsNullOrEmpty(packet.Scene))
            OnPlayerLeaveScene(playerIp);
        else
            OnPlayerEnterScene(playerIp, packet.Scene);
    }

    private void OnPlayerEnterScene(string playerIp, string scene)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Update player's stored scene
        current.UpdateScene(scene);

        // Send updated scene
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip))
        {
            _server.Send(player.Ip, new SceneResponsePacket(current.Name, scene));

            if (InSameScene(current, player))
            {
                // These should just be default right now, but they are about to be sent
                //_server.Send(player.Ip, new PositionResponsePacket(current.Name, current.XPosition, current.YPosition));
                //_server.Send(player.Ip, new AnimationResponsePacket(current.Name, current.Animation));
                //_server.Send(player.Ip, new DirectionResponsePacket(current.Name, current.Direction));

                _server.Send(playerIp, new PositionResponsePacket(player.Name, player.XPosition, player.YPosition));
                _server.Send(playerIp, new AnimationResponsePacket(player.Name, player.Animation));
                _server.Send(playerIp, new DirectionResponsePacket(player.Name, player.Direction));
            }
        }
    }

    private void OnPlayerLeaveScene(string playerIp)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Update player's stored scene
        current.UpdateScene(string.Empty);

        // Send updated scene
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip))
        {
            _server.Send(player.Ip, new SceneResponsePacket(current.Name, string.Empty));
        }
    }

    private void ReceiveSkin(string playerIp, SkinPacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Update player's stored skin
        current.UpdateSkin(packet.Skin);

        // Send updated skin
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip))
        {
            _server.Send(player.Ip, new SkinResponsePacket(current.Name, current.Skin));
        }
    }

    private void ReceiveIntro(string playerIp, IntroPacket packet)
    {
        // Ensure the server doesn't have max number of players
        if (_connectedPlayers.Count >= _maxPlayers)
        {
            Logger.Warn("Player connection rejected: Player limit reached");
            _server.Send(playerIp, new IntroResponsePacket(RefusalType.PlayerLimit));
            return;
        }

        // Ensure there are no duplicate ips
        if (_connectedPlayers.ContainsKey(playerIp))
        {
            Logger.Warn("Player connection rejected: Duplicate ip address");
            _server.Send(playerIp, new IntroResponsePacket(RefusalType.DuplicateIp));
            return;
        }

        // Ensure the protocol version matches
        if (packet.ProtocolVersion != Protocol.VERSION)
        {
            Logger.Warn("Player connection rejected: Protocol version doesn't match");
            _server.Send(playerIp, new IntroResponsePacket(RefusalType.Protocol));
            return;
        }

        // Ensure there are no duplicate names
        if (_connectedPlayers.Values.Any(x => x.Name == packet.PlayerName))
        {
            Logger.Warn("Player connection rejected: Duplicate name");
            _server.Send(playerIp, new IntroResponsePacket(RefusalType.DuplicateName));
            return;
        }

        // Ensure the password is correct
        if (!string.IsNullOrEmpty(_password) && (string.IsNullOrEmpty(packet.Password) || packet.Password != _password))
        {
            Logger.Warn("Player connection rejected: Incorrect password");
            _server.Send(playerIp, new IntroResponsePacket(RefusalType.Password));
            return;
        }

        // Add new connected player
        Logger.Info("Player connection accepted");
        PlayerInfo current = new PlayerInfo(playerIp, packet.PlayerName, packet.TeamNumber);
        _connectedPlayers.Add(playerIp, current);

        // Send connections
        _server.Send(playerIp, new IntroResponsePacket(RefusalType.Accepted));

        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip))
        {
            _server.Send(player.Ip, new JoinResponsePacket(current.Name, current.Team));

            _server.Send(playerIp, new JoinResponsePacket(player.Name, player.Team));
            _server.Send(playerIp, new SceneResponsePacket(player.Name, player.Scene));
            _server.Send(playerIp, new SkinResponsePacket(player.Name, player.Skin));
        }
    }

    private void ReceiveAttack(string playerIp, AttackPacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Send attack info
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip && InSameScene(current, x)))
        {
            _server.Send(player.Ip, new AttackResponsePacket(current.Name, packet.Type, packet.Amount, packet.Victim));
        }
    }

    private void ReceiveEffect(string playerIp, EffectPacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Send effect info
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip && InSameScene(current, x)))
        {
            _server.Send(player.Ip, new EffectResponsePacket(current.Name, packet.Type));
        }
    }

    // TODO: revamp this whole thing
    private void ReceiveProgress(string playerIp, ProgressPacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Update team progress data and send new packet
        string progressId = packet.Id;
        byte progressType = packet.Type;
        byte progressValue = packet.Value;

        // Add the progress to the server data, and if it's new send it to the rest of the players
        if (!Core.getTeamData(current.Team).AddTeamProgress(progressId, progressType, progressValue))
        {
            Logger.ProgressBad($"Received duplicated or inferior progress from {current.Name}: {progressId}, Type {progressType}, Value {progressValue}");
            return;
        }

        if (progressType >= 0 && progressType <= 5)
        {
            // Item
            Logger.ProgressGood($"{(progressValue == 0 ? "Received new" : "Lost an")} item from {current.Name}: {progressId}");
        }
        else if (progressType == 6)
        {
            // Stat
            Logger.ProgressGood($"Received new stat upgrade from {current.Name}: {progressId} level {progressValue + 1}");
        }
        else if (progressType == 7)
        {
            // Skill
            Logger.ProgressGood($"Received new skill from {current.Name}: {progressId}");
        }
        else if (progressType == 8)
        {
            // Map cell
            Logger.ProgressGood($"Received new map cell from {current.Name}: {progressId}");
        }
        else if (progressType == 9)
        {
            // Flag
            Logger.ProgressGood($"Received new flag from {current.Name}: {progressId}");
        }
        else if (progressType == 10)
        {
            // Pers. object
            Logger.ProgressGood($"Received new pers. object from {current.Name}: {progressId}");
        }
        else if (progressType == 11)
        {
            // Teleport
            Logger.ProgressGood($"Received new teleport location from {current.Name}: {progressId}");
        }
        else if (progressType == 12)
        {
            // Church donation
            Logger.ProgressGood($"Received new tear donation from {current.Name}: {progressValue}");
        }
        else if (progressType == 13)
        {
            // Miriam status
            Logger.ProgressGood($"Received new miriam status from {current.Name}: {progressId}");
        }

        // If this is a stat upgrade, might have to do something extra with flask/flaskhealth
        if (progressType == 6)
        {
            if (progressId == "FLASK")
            {
                Logger.Info("Received flask level: " + progressValue);
                byte flaskHealthUpgrades = Core.getTeamData(current.Team).GetTeamProgressValue(6, "FLASKHEALTH");
                progressValue -= flaskHealthUpgrades;
                Logger.Info("Flask level sent out: " + progressValue);
            }
            else if (progressId == "FLASKHEALTH")
            {
                byte flaskUpgrades = Core.getTeamData(current.Team).GetTeamProgressValue(6, "FLASK");
                Logger.Info("Flask level stored: " + flaskUpgrades);
                Logger.Info("Flask level sent: " + (byte)(flaskUpgrades - progressValue));

                foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip && x.Team == current.Team))
                {
                    _server.Send(player.Ip, new ProgressResponsePacket(current.Name, ProgressType.PlayerStat, "FLASK", (byte)(flaskUpgrades - progressValue)));
                }
            }
        }

        // Send progress info
        foreach (var player in _connectedPlayers.Values.Where(x => playerIp != x.Ip && x.Team == current.Team))
        {
            _server.Send(player.Ip, new ProgressResponsePacket(current.Name, (ProgressType)progressType, progressId, progressValue));
        }
    }

    private void ReceivePing(string playerIp, PingPacket packet)
    {
        if (!TryGetPlayer(playerIp, out PlayerInfo current))
            return;

        // Update player's stored ping
        current.UpdatePing(packet.Ping);

        // Send updated position
        var pings = _connectedPlayers.Values.Where(x => playerIp != x.Ip).ToDictionary(x => x.Name, x => x.Ping);
        _server.Send(playerIp, new PingResponsePacket(packet.TimeStamp, pings));
    }
}
