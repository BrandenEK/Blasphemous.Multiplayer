using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Server;
using Blasphemous.Multiplayer.Common;
using Blasphemous.Multiplayer.Server.Models;
using System.Collections.Generic;
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
        sendPlayerConnection(e.ip, false);
        _connectedPlayers.Remove(ip);
        Core.removeUnusedGameData(_connectedPlayers);
    }

    private void OnPacketReceived(string ip, BasePacket packet)
    {
        Logger.Warn($"Received packet of type {packet.GetType().Name} from {ip}");
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

    private const int READ_INTERVAL_MS = 16;

    // =======================================
    // New (old) stuff that I want to refactor
    // =======================================

    private Dictionary<string, PlayerInfo> _connectedPlayers = [];

    public Dictionary<string, PlayerInfo> GetPlayers()
    {
        return _connectedPlayers;
    }

    private PlayerInfo getCurrentPlayer(string ip)
    {
        if (_connectedPlayers.ContainsKey(ip))
            return _connectedPlayers[ip];

        Logger.Warn("Data for " + ip + " has not been created yet!");
        return new PlayerInfo(string.Empty, 1);
    }
}
