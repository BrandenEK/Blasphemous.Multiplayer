using System;

namespace BlasServer
{
    class Core
    {
        // Move to config file
        public static int maxPlayers = 8;

        static void Main(string[] args)
        {
            // Title
            Console.Title = "Blasphemous Multiplayer";
            displayCustom("Blasphemous Multiplayer Server\n", ConsoleColor.Cyan);

            // Get ip address
            //displayMessage("Enter the ip address of the server:");
            //Console.ForegroundColor = ConsoleColor.Green;
            //string ip = Console.ReadLine();

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
