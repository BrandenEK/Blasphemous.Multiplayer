using System.Collections.Generic;
using BlasClient.Structures;

namespace BlasClient
{
    public class ProgressManager
    {
        // Only enabled when processing & applying the queued progress updates
        public static bool updatingProgress;

        private List<ProgressUpdate> queuedProgressUpdates = new List<ProgressUpdate>();
        private static readonly object progressLock = new object();

        public void updateProgress()
        {
            // Make sure player is in a game
            // Set updating progress to true
            // Lock the thread and loop through each queued update
            // Apply progress with switch statement
            // Set updating progress to false
        }

        public void receiveProgress(string id, byte type, byte value)
        {
            lock (progressLock)
            {
                queuedProgressUpdates.Add(new ProgressUpdate(id, type, value));
            }
        }
    }
}
