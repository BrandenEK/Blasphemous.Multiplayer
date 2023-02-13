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
        public static GameData gameData;

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
                displayMessage("Loaded config from " + configPath);
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
                gameData = new GameData();
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
                        gameData.printGameProgress();
                        break;
                    case "players":
                        printPlayers();
                        break;
                }
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
    }
}
