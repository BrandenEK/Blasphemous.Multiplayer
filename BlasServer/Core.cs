using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlasServer
{
    class Core
    {
        public static Config config;
        private static Server server;
        private static Dictionary<byte, GameData> teamGameDatas;

        static void Main(string[] args)
        {
            // Title
            Console.Title = "Blasphemous Multiplayer";
            displayCustom("Blasphemous Multiplayer Server\n", ConsoleColor.Cyan);

            // Load config from file
            string configPath = Environment.CurrentDirectory + "\\multiplayer.cfg";
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                config = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                displayMessage("Creating new config at " + configPath);
            }

            // Create server
            server = new Server();
            if (server.Start())
            {
                teamGameDatas = new Dictionary<byte, GameData>();
                displayMessage("Server has been started at this machine's local ip address");
                CommandLoop();
            }
            else
            {
                displayError("Server failed to start at this machine's local ip address");
            }

            // Exit
            displayCustom("\nPress any key to exit...", ConsoleColor.Gray);
            Console.ReadKey(false);
        }

        public static void displayMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }

        public static void displayError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + error);
        }

        public static void displayCustom(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }

        static void CommandLoop()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                string command = Console.ReadLine().Trim().ToLower();

                switch (command)
                {
                    case "exit":
                        return;
                    case "data":
                        printData();
                        break;
                    case "players":
                        printPlayers();
                        break;
                }
            }
        }

        static void printData()
        {
            displayCustom("Server game data:", ConsoleColor.Cyan);
            foreach (byte team in teamGameDatas.Keys)
            {
                displayCustom("Team " + team + " data:", ConsoleColor.Cyan);
                teamGameDatas[team].PrintTeamProgress();
            }
        }

        static void printPlayers()
        {
            displayCustom("Connected players:", ConsoleColor.Cyan);
            Dictionary<string, PlayerStatus> players = server.getPlayers();
            foreach (string playerName in players.Keys)
            {
                displayMessage(playerName + ": Team " + players[playerName].team);
            }
            displayMessage("");
        }

        public static GameData getTeamData(byte team)
        {
            if (teamGameDatas.ContainsKey(team))
                return teamGameDatas[team];

            displayMessage("Creating new game data for team " + team);
            GameData newData = new GameData();
            teamGameDatas.Add(team, newData);
            return newData;
        }

        public static void removeUnusedGameData(Dictionary<string, PlayerStatus> allPlayers)
        {
            for (byte i = 1; i <= 10; i++)
            {
                if (!teamGameDatas.ContainsKey(i)) continue;

                // If no player is currently on this team, remove the game data
                bool teamExists = false;
                foreach (PlayerStatus player in allPlayers.Values)
                {
                    if (player.team == i)
                    {
                        teamExists = true;
                        break;
                    }
                }
                if (!teamExists)
                {
                    displayMessage("Removing game data for team " + i);
                    teamGameDatas.Remove(i);
                }
            }
        }
    }
}
