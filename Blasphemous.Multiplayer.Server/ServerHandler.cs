using Basalt.Framework.Networking.Serializers;
using Basalt.Framework.Networking.Server;
using Blasphemous.Multiplayer.Server.Models;
using SimpleTCP;
using System.Collections.Generic;

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

        _server = new NetworkServer(new ClassicSerializer());
    }

    public bool Start(int port)
    {
        try
        {
            //server = new SimpleTcpServer();
            //server.ClientConnected += clientConnected;
            //server.ClientDisconnected += clientDisconnected;
            //server.DataReceived += Receive;
            //server.Start(port);
            //server.DelayDisabled = true;
            _server.Start(port);
        }
        catch (System.Net.Sockets.SocketException)
        {
            return false;
        }

        return true;
    }
}
