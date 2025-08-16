using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blasphemous.Multiplayer.Server
{
    class Core
    {
        public static Config config;
        private static Server server;
        private static Dictionary<byte, GameData> teamGameDatas;

        static void Main(string[] args)
        {
            // Title
            Console.Title = "Blasphemous Multiplayer Server";
            Console.WriteLine(string.Empty);

            // Load config from file
            string configPath = Path.Combine(Environment.CurrentDirectory, "multiplayer.cfg");
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                config = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                Logger.Info("Creating new config at " + configPath);
            }

            // Create server
            server = new Server();
            if (server.Start())
            {
                teamGameDatas = new Dictionary<byte, GameData>();
                Logger.Info("Server has been started at this machine's local ip address");
                CommandLoop();
            }
            else
            {
                Logger.Error("Server failed to start at this machine's local ip address");
            }

            // Exit
            Logger.Info("\nPress any key to exit...");
            Console.ReadKey(false);
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
            Logger.Special("Server game data:");
            foreach (byte team in teamGameDatas.Keys)
            {
                Logger.Special("Team " + team + " data:");
                teamGameDatas[team].PrintTeamProgress();
            }
        }

        static void printPlayers()
        {
            Logger.Special("Connected players:");
            Dictionary<string, PlayerStatus> players = server.getPlayers();
            foreach (string playerName in players.Keys)
            {
                Logger.Info(playerName + ": Team " + players[playerName].team);
            }
            Logger.Info("");
        }

        public static GameData getTeamData(byte team)
        {
            if (teamGameDatas.ContainsKey(team))
                return teamGameDatas[team];

            Logger.Info("Creating new game data for team " + team);
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
                    Logger.Info("Removing game data for team " + i);
                    teamGameDatas.Remove(i);
                }
            }
        }
    }
}
