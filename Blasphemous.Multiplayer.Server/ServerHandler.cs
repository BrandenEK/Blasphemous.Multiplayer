using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Serializers;
using Basalt.Framework.Networking.Server;
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

        //_server = new NetworkServer(new ClassicSerializer()
        //    .RegisterPacket<PositionPacket>(0, () => new PositionPacket(0, 0))
        //    .RegisterPacket<ScenePacket>(5, () => new ScenePacket(string.Empty)));

        _server = new NetworkServer(null);

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

        return true;
    }

    private void OnClientConnected(string ip)
    {
        Logger.Info($"Client connected at {ip}");
    }

    private void OnClientDisconnected(string ip)
    {
        Logger.Info($"Client disconnected at {ip}");
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
}
