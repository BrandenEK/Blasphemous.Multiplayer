using System;
using System.Collections.Generic;
using ModdingAPI;

namespace BlasClient
{
    public class MultiplayerCommand : ModCommand
    {
        protected override string CommandName => "multiplayer";

        protected override bool AllowUppercase => true;

        protected override Dictionary<string, Action<string[]>> AddSubCommands()
        {
            return new Dictionary<string, Action<string[]>>()
            {
                { "help", Help },
                { "status", Status },
                { "connect", Connect },
                { "disconnect", Disconnect },
                { "team", Team },
                { "players", Players }
            };
        }

        private void Help(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            Write("Available MULTIPLAYER commands:");
            Write("multiplayer status: Display connection status");
            Write("multiplayer connect SERVER NAME [PASSWORD]: Connect to SERVER with player name as NAME with optional PASSWORD");
            Write("multiplayer disconnect: Disconnect from current server");
            Write("multiplayer team NUMBER: Change to a different team (1-10)");
            Write("multiplayer players: List all connected players in the server");
        }

        private void Status(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            if (Main.Multiplayer.connectedToServer)
            {
                Write("Connected to " + Main.Multiplayer.serverIp);
            }
            else
            {
                Write("Not connected to a server!");
            }
        }

        private void Connect(string[] parameters)
        {
            if (Main.Multiplayer.connectedToServer)
            {
                Write("You are already connected to " + Main.Multiplayer.serverIp);
                return;
            }

            string password = null;
            if (parameters.Length == 3)
            {
                password = parameters[2];
            }
            else if (parameters.Length != 2)
            {
                Write("This command requires either 2 or 3 parameters.  You passed " + parameters.Length);
                return;
            }

            if (!ValidateStringParameter(parameters[1], 1, 16)) return;

            Write($"Attempting to connect to {parameters[0]} as {parameters[1]}...");
            Main.Multiplayer.connectCommand(parameters[0], parameters[1], password);
        }

        private void Disconnect(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            if (Main.Multiplayer.connectedToServer)
            {
                Write("Disconnecting from server");
                Main.Multiplayer.disconnectCommand();
            }
            else
                Write("Not connected to a server!");
        }

        private void Team(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 1) || !ValidateIntParameter(parameters[0], 1, 10, out int newTeam)) return;

            if (newTeam == Main.Multiplayer.playerTeam)
            {
                Write("You are already on team " + newTeam);
            }
            else
            {
                Write("Changing team number to " + newTeam);
                Main.Multiplayer.changeTeam((byte)newTeam);
            }
        }

        private void Players(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            if (!Main.Multiplayer.connectedToServer)
            {
                Write("Not connected to a server!");
                return;
            }

            Write("Connected players:");
            Write(Main.Multiplayer.playerName + ": Team " + Main.Multiplayer.playerTeam);
            foreach (string playerName in Main.Multiplayer.connectedPlayers.Keys)
            {
                Write(playerName + ": Team " + Main.Multiplayer.connectedPlayers[playerName].team);
            }
        }
    }
}
