using System.Collections.Generic;

namespace BlasServer
{
    public class GameData
    {
        private Dictionary<string, byte>[] progressSets;
        private const int numberOfProgressTypes = 18;

        public GameData()
        {
            // Create empty game data
            progressSets = new Dictionary<string, byte>[numberOfProgressTypes];
            for (int i = 0; i < numberOfProgressTypes; i++)
            {
                progressSets[i] = new Dictionary<string, byte>();
            }
        }

        public bool addPlayerProgress(string progressId, byte progressType, byte progressValue)
        {
            Dictionary<string, byte> currentProgressSet = progressSets[progressType];

            if (currentProgressSet.ContainsKey(progressId))
            {
                // This progress id has already been obtained
                // Change this to instead update the value if it is higher
                return false;
            }
            else
            {
                // This is a new progress
                currentProgressSet.Add(progressId, progressValue);
                return true;
            }
        }

        public byte checkPlayerProgress(string progressId, byte progressType)
        {
            Dictionary<string, byte> currentProgressSet = progressSets[progressType];
            return currentProgressSet.ContainsKey(progressId) ? currentProgressSet[progressId] : (byte)0;
        }

        public void printGameProgress()
        {
            for (int i = 0; i < numberOfProgressTypes; i++)
            {
                Core.displayMessage("---Progress type " + i + "---");
                foreach (string id in progressSets[i].Keys)
                {
                    Core.displayMessage(id + ": " + progressSets[i][id]);
                }
            }
        }
    }
}
