using System.Collections.Generic;

namespace BlasServer
{
    public class GameData
    {
        private Dictionary<string, byte>[] progressSets;
        public const int numberOfProgressTypes = 14;

        public GameData()
        {
            // Create empty game data
            progressSets = new Dictionary<string, byte>[numberOfProgressTypes];
            for (int i = 0; i < numberOfProgressTypes; i++)
            {
                progressSets[i] = new Dictionary<string, byte>();
            }
        }

        // Adds the player progress to the server data and determines whether to send it to the rest of the players or not
        public bool addPlayerProgress(string progressId, byte progressType, byte progressValue)
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
        public Dictionary<string, byte> getProgressSet(int progressType)
        {
            if (progressType >= 0 && progressType < progressSets.Length)
            {
                return progressSets[progressType];
            }
            return null;
        }

        public void printGameProgress()
        {
            for (int i = 0; i < numberOfProgressTypes; i++)
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
