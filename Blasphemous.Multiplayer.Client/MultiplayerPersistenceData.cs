using Blasphemous.ModdingAPI.Persistence;
using System.Collections.Generic;

namespace Blasphemous.Multiplayer.Client
{
    [System.Serializable]
    public class MultiplayerPersistenceData : SaveData
    {
        public MultiplayerPersistenceData() : base("ID_MULTIPLAYER") { }

        public List<string> interactedPersistenceObjects;
    }
}
