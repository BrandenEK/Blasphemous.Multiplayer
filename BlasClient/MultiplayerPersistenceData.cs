using System.Collections.Generic;
using Framework.Managers;

namespace BlasClient
{
    [System.Serializable]
    public class MultiplayerPersistenceData : PersistentManager.PersistentData
    {
        public MultiplayerPersistenceData() : base("ID_MULTIPLAYER") { }

        public List<string> interactedPersistenceObjects;
    }
}
