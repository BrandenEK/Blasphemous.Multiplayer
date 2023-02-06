namespace BlasClient.Structures
{
    [System.Serializable]
    public class SyncSettings
    {
        public bool inventoryItems;
        public bool playerStats;
        public bool swordSkills;
        public bool worldState; // Split into further sections maybe
        public bool mapCells;

        // Default sync settings
        public SyncSettings()
        {
            inventoryItems = true;
            playerStats = true;
            swordSkills = true;
            worldState = true;
            mapCells = true;
        }
    }
}
