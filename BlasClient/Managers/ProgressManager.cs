using UnityEngine;
using System.Collections.Generic;
using Framework.Managers;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes.Logic;
using BlasClient.Structures;
using BlasClient.Data;

namespace BlasClient.Managers
{
    public class ProgressManager
    {
        // Only enabled when processing & applying the queued progress updates
        public static bool updatingProgress;
        public enum ProgressType { Bead, Prayer, Relic, Heart, Collectible, QuestItem, PlayerStat, SwordSkill, MapCell, Flag, PersistentObject, Teleport }

        private List<ProgressUpdate> queuedProgressUpdates = new List<ProgressUpdate>();
        private PersistentObject[] scenePersistentObjects = new PersistentObject[0];
        private static readonly object progressLock = new object();

        public void sceneLoaded(string scene)
        {
            scenePersistentObjects = Object.FindObjectsOfType<PersistentObject>();
            foreach (PersistentObject persistence in scenePersistentObjects)
            {
                int objectSceneIdx = PersistentStates.getObjectSceneIndex(scene, persistence.GetPersistenID());
                string objectSceneId = scene + "~" + objectSceneIdx;

                // This object does not even sync or hasn't been interacted with yet
                if (objectSceneIdx < 0 || !Main.Multiplayer.checkPersistentObject(objectSceneId)) continue;

                // Calling setPersistence() with null data means to play instant animation
                persistence.SetCurrentPersistentState(null, false, null);
            }
        }

        public void updateProgress()
        {
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
            switch ((ProgressType)progress.type)
            {
                case ProgressType.Bead:
                    if (Main.Multiplayer.config.syncSettings.inventoryItems)
                    {
                        if (progress.value == 1)
                            Core.InventoryManager.RemoveRosaryBead(progress.id);
                        else
                            Core.InventoryManager.AddRosaryBead(progress.id);
                    }
                    return;
                case ProgressType.Prayer:
                    if (Main.Multiplayer.config.syncSettings.inventoryItems)
                        Core.InventoryManager.AddPrayer(progress.id);
                    return;
                case ProgressType.Relic:
                    if (Main.Multiplayer.config.syncSettings.inventoryItems)
                        Core.InventoryManager.AddRelic(progress.id);
                    return;
                case ProgressType.Heart:
                    if (Main.Multiplayer.config.syncSettings.inventoryItems)
                        Core.InventoryManager.AddSword(progress.id);
                    return;
                case ProgressType.Collectible:
                    if (Main.Multiplayer.config.syncSettings.inventoryItems)
                        Core.InventoryManager.AddCollectibleItem(progress.id);
                    return;
                case ProgressType.QuestItem:
                    if (Main.Multiplayer.config.syncSettings.inventoryItems)
                    {
                        if (progress.value == 1)
                            Core.InventoryManager.RemoveQuestItem(progress.id);
                        else
                            Core.InventoryManager.AddQuestItem(progress.id);
                    }
                    return;
                case ProgressType.PlayerStat:
                    if (Main.Multiplayer.config.syncSettings.playerStats)
                        upgradeStat(progress.id, progress.value);
                    return;
                case ProgressType.SwordSkill:
                    if (Main.Multiplayer.config.syncSettings.swordSkills)
                        Core.SkillManager.UnlockSkill(progress.id);
                    return;
                case ProgressType.MapCell:
                    if (Main.Multiplayer.config.syncSettings.mapCells)
                        Core.NewMapManager.RevealCellInPosition(new Vector2(int.Parse(progress.id), 0));
                    return;
                case ProgressType.Flag:
                    if (Main.Multiplayer.config.syncSettings.worldState)
                        Core.Events.SetFlag(progress.id, progress.value == 0, false);
                    return;
                case ProgressType.PersistentObject:
                    if (Main.Multiplayer.config.syncSettings.worldState)
                        updatePersistentObject(progress.id);
                    return;
                case ProgressType.Teleport:
                    if (Main.Multiplayer.config.syncSettings.worldState)
                        Core.SpawnManager.SetTeleportActive(progress.id, true);
                    return;

                // Church donations
                default:
                    Main.UnityLog("Error: Progress type doesn't exist: " + progress.type); return;
            }
        }

        // Called when receiving a stat upgrade
        public void upgradeStat(string stat, byte level)
        {
            Attribute attribute;
            switch (stat)
            {
                case "LIFE": attribute = Core.Logic.Penitent.Stats.Life; break;
                case "FERVOUR": attribute = Core.Logic.Penitent.Stats.Fervour; break;
                case "STRENGTH": attribute = Core.Logic.Penitent.Stats.Strength; break;
                case "MEACULPA": attribute = Core.Logic.Penitent.Stats.MeaCulpa; break;
                case "BEADSLOTS": attribute = Core.Logic.Penitent.Stats.BeadSlots; break;
                case "FLASK": attribute = Core.Logic.Penitent.Stats.Flask; break;
                case "FLASKHEALTH": attribute = Core.Logic.Penitent.Stats.FlaskHealth; break;
                default:
                    Main.UnityLog("Error: Unknown stat received - " + stat);
                    return;
            }

            while (attribute.GetUpgrades() < level)
            {
                attribute.Upgrade();
            }
        }

        // Called when interacting with pers. object - determine whether to send it or not
        public void usePersistentObject(string persistentId)
        {
            string scene = Core.LevelManager.currentLevel.LevelName;
            int objectSceneIdx = PersistentStates.getObjectSceneIndex(scene, persistentId);
            string objectSceneId = scene + "~" + objectSceneIdx;

            // Make sure this pers. object should sync & isn't already activated
            if (objectSceneIdx < 0 || Main.Multiplayer.checkPersistentObject(objectSceneId)) return;

            // Update save game data & send this object
            Main.Multiplayer.addPersistentObject(objectSceneId);
            if (Main.Multiplayer.config.syncSettings.worldState)
                Main.Multiplayer.obtainedGameProgress(objectSceneId, ProgressType.PersistentObject, 0);
        }

        // When receiving a pers. object update, the object is immediately updated
        private void updatePersistentObject(string objectSceneId)
        {
            Main.Multiplayer.addPersistentObject(objectSceneId);

            string objectScene = objectSceneId.Substring(0, objectSceneId.IndexOf('~'));
            int objectSceneIdx = int.Parse(objectSceneId.Substring(objectSceneId.IndexOf('~') + 1));
            string objectPersistentId = PersistentStates.getObjectPersistentId(objectScene, objectSceneIdx);

            if (Core.LevelManager.currentLevel.LevelName != objectScene || objectPersistentId == null)
                return;

            // Player just received a pers. object in the same scene - find it and set value immediately
            foreach (PersistentObject persistentObject in scenePersistentObjects)
            {
                try
                {
                    if (persistentObject.GetPersistenID() == objectPersistentId)
                    {
                        // Calling getPersistence() with "use" means to play used animation
                        persistentObject.GetCurrentPersistentState("use", false);
                    }
                }
                catch (System.NullReferenceException)
                {
                    Main.UnityLog("Error: Failed to get persistent id of object");
                }
            }
        }
    }
}
