using System;
using System.IO;
using Newtonsoft.Json;

namespace BlasServer
{
    class Core
    {
        public static Config config;

        static void Main(string[] args)
        {
            // Title
            Console.Title = "Blasphemous Multiplayer";
            displayCustom("Blasphemous Multiplayer Server\n", ConsoleColor.Cyan);

            // Load config from file
            string configPath = Environment.CurrentDirectory + "/multiplayer.cfg";
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<Config>(json);
                displayMessage("Loaded config from " + configPath);
            }
            else
            {
                config = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
                displayMessage("Creating new config at " + configPath);
            }

            // Create server
            Server server = new Server();
            if (server.Start())
            {
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
                if (command == "exit")
                    break;
            }
        }
    }
}
