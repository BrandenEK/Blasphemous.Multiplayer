using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlasClient
{
    public class Multiplayer
    {
        private Client client;

        public void update()
        {

        }

        public Multiplayer()
        {
            client = new Client("localhost", "test");
            client.Connect();

            client.sendPlayerName();
        }
    }
}
