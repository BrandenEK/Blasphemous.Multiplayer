using System.Collections.Generic;

namespace BlasServer
{
    public class GameData
    {
        public const int NUMBER_OF_PROGRESS_TYPES = 14;

        private readonly Dictionary<string, byte>[] progressSets;

        public GameData()
        {
            // Create empty game data
            progressSets = new Dictionary<string, byte>[NUMBER_OF_PROGRESS_TYPES];
            for (int i = 0; i < NUMBER_OF_PROGRESS_TYPES; i++)
            {
                progressSets[i] = new Dictionary<string, byte>();
            }
        }

        // Adds the player progress to the server data and determines whether to send it to the rest of the players or not
        public bool AddTeamProgress(string progressId, byte progressType, byte progressValue)
        {
            Dictionary<string, byte> currentProgressSet = progressSets[progressType];

            if (!currentProgressSet.ContainsKey(progressId))
            {
                // This is a new progress
                currentProgressSet.Add(progressId, progressValue);
                return true;
            }

            byte currentValue = currentProgressSet[progressId];
            if (progressValue > currentValue)
            {
                // The new progress has a higher value than the server
                currentProgressSet[progressId] = progressValue;
                return true;
            }

            // The new progress was less than the server
            return false;
        }

        // Used by the server to send all of the server data on player connection
        public Dictionary<string, byte> GetTeamProgressSet(int progressType)
        {
            if (progressType >= 0 && progressType < progressSets.Length)
            {
                return progressSets[progressType];
            }
            
            throw new System.ArgumentOutOfRangeException("Tried to get an invalid team progress set: " + progressType);
        }

        public byte GetTeamProgressValue(int progressType, string progressId)
        {
            Dictionary<string, byte> currentProgressSet = GetTeamProgressSet(progressType);

            if (currentProgressSet.TryGetValue(progressId, out byte value))
            {
                return value;
            }
            else
            {
                return 0;
            }
        }

        public void PrintTeamProgress()
        {
            for (int i = 0; i < progressSets.Length; i++)
            {
                if (progressSets[i].Count == 0) continue;

                Core.displayMessage("---Progress type " + i + "---");
                foreach (string id in progressSets[i].Keys)
                {
                    Core.displayMessage(id + ": " + progressSets[i][id]);
                }
            }
            Core.displayMessage("");
        }
    }
}
