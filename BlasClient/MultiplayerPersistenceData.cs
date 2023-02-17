using System.Collections.Generic;
using ModdingAPI;

namespace BlasClient
{
    [System.Serializable]
    public class MultiplayerPersistenceData : ModPersistentData
    {
        public MultiplayerPersistenceData() : base("ID_MULTIPLAYER") { }

        public List<string> interactedPersistenceObjects;
    }
}
