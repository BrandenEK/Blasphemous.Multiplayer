
namespace BlasClient.ProgressSync
{
    public class ProgressUpdate
    {
        private readonly string id;
        public string Id => id;

        private readonly ProgressType type;
        public ProgressType Type => type;

        private readonly byte value;
        public byte Value => value;

        public ProgressUpdate(string id, ProgressType type, byte value)
        {
            this.id = id;
            this.type = type;
            this.value = value;
        }

        public bool ShouldSyncProgress(Config config)
        {
            return type switch
            {
                ProgressType.Bead or
                ProgressType.Prayer or
                ProgressType.Relic or
                ProgressType.Heart or
                ProgressType.Collectible or
                ProgressType.QuestItem => config.syncSettings.inventoryItems,

                ProgressType.PlayerStat => config.syncSettings.playerStats,

                ProgressType.SwordSkill => config.syncSettings.swordSkills,

                ProgressType.MapCell => config.syncSettings.mapCells,

                ProgressType.Flag or
                ProgressType.PersistentObject or
                ProgressType.Teleport or
                ProgressType.ChurchDonation or
                ProgressType.MiriamStatus => config.syncSettings.worldState,

                _ => false
            };
        }
    }
}
