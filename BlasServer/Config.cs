namespace BlasServer
{
    [System.Serializable]
    public class Config
    {
        public int serverPort;
        public int maxPlayers;

        // Default config
        public Config()
        {
            serverPort = 8989;
            maxPlayers = 8;
        }
    }
}
