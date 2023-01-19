using System;

namespace BlasClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Blasphemous client");

            Console.WriteLine("Enter ip:");
            string ip = Console.ReadLine();

            Client client = new Client(ip, "Test");
            if (client.Connect())
            {
                Console.WriteLine("Connected!");
                // send test data
                client.sendPlayerUpdate();
                client.sendPlayerUpdate();
            }
            else
            {
                Console.WriteLine("Faield to connect!");
            }


            Console.WriteLine("Press any key to exit");
            Console.ReadKey(false);
        }
    }
}
