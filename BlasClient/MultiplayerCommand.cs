using System;
using System.Collections.Generic;
using ModdingAPI.Commands;

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
            // Already connected
            if (Main.Multiplayer.connectedToServer)
            {
                Write("You are already connected to " + Main.Multiplayer.serverIp);
                return;
            }

            // Too few parameters
            if (parameters.Length < 2)
            {
                Write("This command requires either 2 or 3 parameters.  You passed " + parameters.Length);
                return;
            }

            string name = "";
            string password = null;
            int passIdx = -1;

            // Name has a space and spans multiple parameters
            if (parameters[1].StartsWith("\""))
            {
                // Find the ending index of the name
                for (int i = parameters.Length - 1; i >= 1; i--)
                {
                    if (parameters[i].EndsWith("\""))
                    {
                        passIdx = i + 1;
                        break;
                    }
                }

                // Verify the ending quote exists
                if (passIdx == -1)
                {
                    Write("Invalid syntax!");
                    return;
                }

                // Build up the name
                for (int i = 1; i < passIdx; i++)
                {
                    name += parameters[i] + " ";
                }
                name = name.Substring(1, name.Length - 3);
            }
            else
            {
                // Name is only one word
                name = parameters[1];
                passIdx = 2;
            }

            // Too many parameters
            if (parameters.Length > passIdx + 1)
            {
                Write("This command requires either 2 or 3 parameters.  You passed " + parameters.Length);
                return;
            }

            if (parameters.Length > passIdx)
            {
                password = parameters[passIdx];
            }

            if (!ValidateStringParameter(name, 1, 16)) return;

            Write($"Attempting to connect to {parameters[0]} as {name}...");
            Main.Multiplayer.connectCommand(parameters[0], name, password);
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
            foreach (string playerName in Main.Multiplayer.playerList.getAllPlayers())
            {
                Write(playerName + ": Team " + Main.Multiplayer.playerList.getPlayerTeam(playerName));
            }
        }
    }
}
