using Blasphemous.CheatConsole;
using Blasphemous.ModdingAPI;
using Blasphemous.Multiplayer.Client.PvP.Models;
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
                { "help", Obsolete },
                { "status", Obsolete },
                { "connect", Obsolete },
                { "disconnect", Obsolete },
                { "players", Obsolete },
#if DEBUG
                { "damage", Damage },
#endif
            };
        }

        private void Obsolete(string[] parameters)
        {
            Write("The multiplayer command has been removed.  Use the keybinding to open the new connection menu.");
        }

//        private void Help(string[] parameters)
//        {
//            if (!ValidateParameterList(parameters, 0))
//                return;

//            Write("Available MULTIPLAYER commands:");
//            Write("multiplayer status: Display connection status");
//            Write("multiplayer connect SERVER NAME [PASSWORD]: Connect to SERVER with player name as NAME with optional PASSWORD");
//            Write("multiplayer disconnect: Disconnect from current server");
//            Write("multiplayer players: List all connected players in the server");
//#if DEBUG
//            Write("multiplayer damage TYPE AMOUNT: Simulates receiving a pvp attack");
//#endif
//        }

        private void Damage(string[] parameters)
        {
            if (!ValidateParameterList(parameters, 2))
                return;

            AttackType type = (AttackType)Enum.Parse(typeof(AttackType), parameters[0]);
            ValidateIntParameter(parameters[1], 0, 255, out int amount);

            ModLog.Warn($"Testing attack {type} with damage {amount}");
            Main.Multiplayer.AttackManager.DamagePlayer_Internal(type, (byte)amount);
        }
    }
}
