using UnityEngine;
using System.Collections.Generic;
using Framework.Managers;
using Framework.FrameworkCore;
using BlasClient.Structures;
using Tools.Level.Interactables;
using Tools.Level.Actionables;
using Gameplay.GameControllers.Environment.MovingPlatforms;

namespace BlasClient.Managers
{
    public class ProgressManager
    {
        // Only enabled when processing & applying the queued progress updates
        public static bool updatingProgress;

        private List<ProgressUpdate> queuedProgressUpdates = new List<ProgressUpdate>();
        private PersistentObject[] scenePersistentObjects = new PersistentObject[0];
        private static readonly object progressLock = new object();

        public void sceneLoaded()
        {
            scenePersistentObjects = Object.FindObjectsOfType<PersistentObject>();
            foreach (PersistentObject persistence in scenePersistentObjects)
            {
                if (Main.Multiplayer.checkPersistentObject(persistence.GetPersistenID()))
                {
                    // Calling setPersistence() with null data means to play instant animation
                    persistence.SetCurrentPersistentState(null, false, null);
                }
            }
        }

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
                case 13:
                    Core.SkillManager.UnlockSkill(progress.id); return;
                case 14:
                    Core.Events.SetFlag(progress.id, true, false); return;
                case 15:
                    updatePersistentObject(progress.id); return;
                case 16:
                    Core.SpawnManager.SetTeleportActive(progress.id, true); return;
                case 17:
                    Core.NewMapManager.RevealCellInPosition(new Vector2(int.Parse(progress.id), 0)); return;

                // Church donations
                default:
                    Main.UnityLog("Error: Progress type doesn't exist: " + progress.type); return;
            }
        }

        // Called when interacting with pers. object - determine whether to send it or not
        public void usePersistentObject(string persistentId)
        {
            if (!updatingProgress && StaticObjects.GetPersistenceState(persistentId) != null && !Main.Multiplayer.checkPersistentObject(persistentId))
            {
                // Update save game data & send this object
                Main.Multiplayer.addPersistentObject(persistentId);
                Main.Multiplayer.obtainedGameProgress(persistentId, 15, 0);
            }
        }

        // When receiving a pers. object update, the object is immediately updated
        private void updatePersistentObject(string persistentId)
        {
            Main.Multiplayer.addPersistentObject(persistentId);

            PersistenceState persistence = StaticObjects.GetPersistenceState(persistentId);
            if (persistence == null || Core.LevelManager.currentLevel.LevelName != persistence.scene)
                return;

            // Player just received a pers. object in the same scene - find it and set value immediately
            foreach (PersistentObject persistentObject in scenePersistentObjects)
            {
                if (persistentObject.GetPersistenID() == persistentId)
                {
                    // Calling getPersistence() with "use" means to play used animation
                    persistentObject.GetCurrentPersistentState("use", false);
                }
            }
        }
    }
}
