namespace BlasClient.Structures
{
    [System.Serializable]
    public class Config
    {
        public int serverPort;
        public float notificationDisplaySeconds;
        public bool displayNametags;
        public bool displayOwnNametag;
        public bool showPlayersOnMap;
        public int team;
        public SyncSettings syncSettings;

        // Default config
        public Config()
        {
            serverPort = 25565;
            notificationDisplaySeconds = 4f;
            displayNametags = true;
            displayOwnNametag = true;
            showPlayersOnMap = true;
            team = 1;
            syncSettings = new SyncSettings();
        }
    }
}
