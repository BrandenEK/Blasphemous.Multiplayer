using System;

namespace BlasServer
{
    class Core
    {
        static void Main(string[] args)
        {
            // Title
            Console.Title = "Blasphemous Multiplayer";
            displayCustom("Blasphemous Multiplayer Server\n", ConsoleColor.Cyan);

            // Get ip address
            displayMessage("Enter the ip address of the server:");
            Console.ForegroundColor = ConsoleColor.Green;
            string ip = Console.ReadLine();

            // Create server
            Server server = new Server(ip);
            if (server.Start())
            {
                displayMessage($"\nServer has been started at '{ip}'");
                CommandLoop();
            }
            else
            {
                displayError($"\nServer failed to start at '{ip}'");
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
