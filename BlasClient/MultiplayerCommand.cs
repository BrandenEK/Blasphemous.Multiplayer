using System;
using System.Collections.Generic;
using Gameplay.UI.Console;

namespace BlasClient
{
    public class MultiplayerCommand : ConsoleCommand
    {
        public override void Execute(string command, string[] parameters)
        {
            List<string> paramList;
            string subcommand = GetSubcommand(parameters, out paramList);
            if (command != null && command == "multiplayer")
            {
                processMultiplayer(subcommand, paramList);
            }
        }

        private void processMultiplayer(string command, List<string> parameters)
        {
            if (command == null)
            {
                Console.Write("Command unknown, use multiplayer help");
                return;
            }
            string fullCommand = "multiplayer " + command;

            if (command == "help" && ValidateParams(fullCommand, 0, parameters))
            {
                Console.Write("Available MULTIPLAYER commands:");
                Console.Write("multiplayer status: Display connection status");
                Console.Write("multiplayer connect SERVER NAME [PASSWORD]: Connect to SERVER with player name as NAME with optional PASSWORD");
                Console.Write("multiplayer disconnect: Disconnect from current server");
                Console.Write("multiplayer team NUMBER: Change to a different team (1-10)");
                Console.Write("multiplayer players: List all connected players in the server");
            }
            else if (command == "status" && ValidateParams(fullCommand, 0, parameters))
            {
                if (Main.Multiplayer.connectedToServer)
                {
                    Console.Write("Connected to " + Main.Multiplayer.serverIp);
                }
                else
                {
                    Console.Write("Not connected to a server!");
                }
            }
            else if (command == "connect")
            {
                if (Main.Multiplayer.connectedToServer)
                {
                    Console.Write("You are already connected to " + Main.Multiplayer.serverIp);
                    return;
                }

                string password = null;
                if (parameters.Count == 3)
                {
                    password = parameters[2];
                }
                else if (parameters.Count != 2)
                {
                    Console.Write("The command 'connect' requires either 2 or 3 parameters.  You passed " + parameters.Count);
                    return;
                }

                if (parameters[1].Length > 16)
                {
                    Console.Write("Player name can not be more than 16 characters");
                    return;
                }

                Console.Write($"Attempting to connect to {parameters[0]} as {parameters[1]}...");
                Main.Multiplayer.connectCommand(parameters[0], parameters[1], password);
            }
            else if (command == "disconnect" && ValidateParams(fullCommand, 0, parameters))
            {
                if (Main.Multiplayer.connectedToServer)
                {
                    Console.Write("Disconnecting from server");
                    Main.Multiplayer.disconnectCommand();
                }
                else
                    Console.Write("Not connected to a server!");
            }
            else if (command == "team" && ValidateParams(fullCommand, 1, parameters))
            {
                if (byte.TryParse(parameters[0], out byte newTeam) && newTeam > 0 && newTeam <= 10)
                {
                    if (newTeam == Main.Multiplayer.playerTeam)
                    {
                        Console.Write("You are already on team " + newTeam);
                    }
                    else
                    {
                        Console.Write("Changing team number to " + newTeam);
                        Main.Multiplayer.changeTeam(newTeam);
                    }
                }
                else
                {
                    Console.Write("Team number must be between 1 and 10");
                }
            }
            else if (command == "players" && ValidateParams(fullCommand, 0, parameters))
            {
                if (!Main.Multiplayer.connectedToServer)
                {
                    Console.Write("Not connected to a server!");
                    return;
                }

                Console.Write("Connected players:");
                Console.Write(Main.Multiplayer.playerName + ": Team " + Main.Multiplayer.playerTeam);
                foreach (string playerName in Main.Multiplayer.connectedPlayers.Keys)
                {
                    Console.Write(playerName + ": Team " + Main.Multiplayer.connectedPlayers[playerName].team);
                }
            }
        }

        public override List<string> GetNames()
        {
            return new List<string> { "multiplayer" };
        }

        public override bool ToLowerAll() { return false; }

        public override bool HasLowerParameters() { return false; }
    }
}
