using Basalt.Framework.Networking.Serializers;
using Basalt.Framework.Networking.Server;
using Blasphemous.Multiplayer.Server.TempCommon;
using System.Collections.Generic;
using System.Net.Sockets;

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

    // TODO: temp until background thread is used
    public void Refresh()
    {
        Logger.Info("Reading and writing data...");
        _server.Receive();

        var ser = new ReflectionSerializer()
            .AddPacketSerializer(5, () => new PositionPacket(0, 0))
            .AddPacketSerializer(6, () => new ScenePacket(string.Empty));

        var bytes = new List<byte>();
        bytes.AddRange(ser.Serialize(new PositionPacket(95, 33)));
        bytes.AddRange(ser.Serialize(new ScenePacket("D17Z01S01")));
        bytes.AddRange(ser.Serialize(new PositionPacket(94948, -777)));

        foreach (var x in ser.Deserialize(bytes.ToArray()))
        {
            switch (x)
            {
                case PositionPacket p:
                    Logger.Error(p.X + ", " + p.Y);
                    break;
                case ScenePacket s:
                    Logger.Error("Scene: " + s.Scene);
                    break;
            }
        }

        _server.Update();
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
}
