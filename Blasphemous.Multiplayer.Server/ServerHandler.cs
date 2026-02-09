using Basalt.Framework.Networking;
using Basalt.Framework.Networking.Serializers;
using Basalt.Framework.Networking.Server;
using Blasphemous.Multiplayer.Server.TempCommon;
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

        _server = new NetworkServer(new ClassicSerializer()
            .AddPacketSerializer<PositionPacket>(5, new GenericPacketSerializer(() => new PositionPacket(0, 0))));

        _server.OnClientConnected += OnClientConnected;
        _server.OnClientDisconnected += OnClientDisconnected;
        _server.OnPacketReceived += OnPacketReceived;
        StartReadThread();
    }

    public bool Start(int port)
    {
        try
        {
            _server.Start(port);
        }
        catch (SocketException)
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

    private void OnPacketReceived(string ip, Basalt.Framework.Networking.BasePacket packet)
    {
        Logger.Warn($"Received packet of type {packet.GetType().Name} from {ip}");
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
                try
                {
                    _server.Receive();
                }
                catch (NetworkException ex) // TODO: clean up these exceptions
                {
                    Logger.Error($"Error during deserialization: {ex.Message}");
                }
                catch (System.Exception ex)
                {
                    Logger.Error($"Error during deserialization: {ex}");
                }

                _server.Update();
            }

            Thread.Sleep(READ_INTERVAL_MS);
        }
    }

    private const int READ_INTERVAL_MS = 16;
}
