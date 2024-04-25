using Blasphemous.CheatConsole;
using Blasphemous.Multiplayer.Client.PvP;
using System;
using System.Collections.Generic;

namespace Blasphemous.Multiplayer.Client
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
                { "players", Players },
#if DEBUG
                { "damage", Damage },
#endif
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
#if DEBUG
            Write("multiplayer damage TYPE AMOUNT: Simulates receiving a pvp attack");
#endif
        }

        private void Status(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            if (Main.Multiplayer.NetworkManager.IsConnected)
            {
                Write("Connected to " + Main.Multiplayer.NetworkManager.ServerIP);
            }
            else
            {
                Write("Not connected to a server!");
            }
        }

        private void Connect(string[] parameters)
        {
            // Already connected
            if (Main.Multiplayer.NetworkManager.IsConnected)
            {
                Write("You are already connected to " + Main.Multiplayer.NetworkManager.ServerIP);
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

            bool result = Main.Multiplayer.NetworkManager.Connect(parameters[0], name, password);
            Write(result ? $"Successfully connected to {parameters[0]} as {name}" : $"Failed to connect to {parameters[0]}");
        }

        private void Disconnect(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 0)) return;

            if (Main.Multiplayer.NetworkManager.IsConnected)
            {
                Write("Disconnecting from server");
                Main.Multiplayer.NetworkManager.Disconnect();
            }
            else
                Write("Not connected to a server!");
        }

        private void Team(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 1) || !ValidateIntParameter(parameters[0], 1, 10, out int newTeam)) return;

            if (newTeam == Main.Multiplayer.PlayerTeam)
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

            if (!Main.Multiplayer.NetworkManager.IsConnected)
            {
                Write("Not connected to a server!");
                return;
            }

            Write("Connected players:");
            Write(Main.Multiplayer.PlayerName + ": Team " + Main.Multiplayer.PlayerTeam);
            foreach (Players.PlayerStatus player in Main.Multiplayer.OtherPlayerManager.AllConnectedPlayers)
            {
                Write(player.Name + ": Team " + player.Team);
            }
        }

        private void Damage(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 2)) return;

            AttackType type = (AttackType)Enum.Parse(typeof(AttackType), parameters[0]);
            ValidateIntParameter(parameters[1], 0, 255, out int amount);

            Main.Multiplayer.LogWarning($"Testing attack {type} with damage {amount}");
            Main.Multiplayer.AttackManager.DamagePlayer_Internal(type, (byte)amount);
        }
    }
}
