using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Server;
using Blasphemous.Multiplayer.Common;
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
}
