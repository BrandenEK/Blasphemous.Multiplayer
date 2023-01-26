using System.Collections.Generic;
using Framework.Managers;
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
            if (!Main.Multiplayer.inLevel)
                return;

            lock (progressLock)
            {
                updatingProgress = true;

                for (int i = 0; i < queuedProgressUpdates.Count; i++)
                {
                    applyProgress(queuedProgressUpdates[i]);
                }
                queuedProgressUpdates.Clear();

                updatingProgress = false;
            }
        }

        public void receiveProgress(string id, byte type, byte value)
        {
            lock (progressLock)
            {
                Main.UnityLog("Received new game progress: " + id);
                queuedProgressUpdates.Add(new ProgressUpdate(id, type, value));
            }
        }

        private void applyProgress(ProgressUpdate progress) // TODO - Check for value to determine whether to remove or add
        {
            switch (progress.type)
            {
                case 0:
                    Core.InventoryManager.AddRosaryBead(progress.id); return;
                case 1:
                    Core.InventoryManager.AddPrayer(progress.id); return;
                case 2:
                    Core.InventoryManager.AddRelic(progress.id); return;
                case 3:
                    Core.InventoryManager.AddSword(progress.id); return;
                case 4:
                    Core.InventoryManager.AddCollectibleItem(progress.id); return;
                case 5:
                    Core.InventoryManager.AddQuestItem(progress.id); return;
                case 6:
                    return;
                case 7:

                // Flags 
                // Persistent objects
                // Unlocked teleports
                // Activated prie dieus
                // Church donations
                // Unlocked skills
                default:
                    Main.UnityLog("Error: Progress type doesn't exist: " + progress.type); return;
            }
        }
    }
}
