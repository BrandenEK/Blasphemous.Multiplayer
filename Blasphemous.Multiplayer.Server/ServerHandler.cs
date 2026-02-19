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

        // Make sure this client was actually connected, not just rejected from server
        if (!_connectedPlayers.ContainsKey(ip))
            return;

        // Send that this player has disconnected & remove them
        //sendPlayerConnection(e.ip, false);
        _connectedPlayers.Remove(ip);
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

            default:
                Logger.Error("TEMP: Dont know what to do with this packet yet");
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
            OnPlayerEnterScene(playerIp, packet.Scene);
        else
            OnPlayerLeaveScene(playerIp);
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
}
