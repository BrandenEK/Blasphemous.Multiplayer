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

        // TODO - Check for value to determine whether to remove or add
        // TODO - For stats - value will contain the current level of the stat
        private void applyProgress(ProgressUpdate progress)
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
                    Core.Logic.Penitent.Stats.Life.Upgrade();
                    Core.Logic.Penitent.Stats.Life.SetToCurrentMax(); return;
                case 7:
                    Core.Logic.Penitent.Stats.Fervour.Upgrade();
                    Core.Logic.Penitent.Stats.Fervour.SetToCurrentMax(); return;
                case 8:
                    Core.Logic.Penitent.Stats.Strength.Upgrade(); return;
                case 9:
                    Core.Logic.Penitent.Stats.MeaCulpa.Upgrade(); return;
                case 10:
                    Core.Logic.Penitent.Stats.BeadSlots.Upgrade(); return;
                case 11:
                    Core.Logic.Penitent.Stats.Flask.Upgrade();
                    Core.Logic.Penitent.Stats.Flask.SetToCurrentMax(); return;
                case 12:
                    Core.Logic.Penitent.Stats.FlaskHealth.Upgrade(); return;

                // Flags 
                // Persistent objects
                // Unlocked teleports
                // Activated prie dieus
                // Church donations
                // Unlocked skills
                // Map
                default:
                    Main.UnityLog("Error: Progress type doesn't exist: " + progress.type); return;
            }
        }
    }
}
