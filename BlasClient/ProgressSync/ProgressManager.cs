using BlasClient.Data;
using BlasClient.ProgressSync.Helpers;
using Framework.FrameworkCore;
using Framework.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace BlasClient.ProgressSync
{
    public class ProgressManager
    {
        // Only enabled when processing & applying the queued progress updates
        public bool CurrentlyUpdatingProgress { get; private set; }

        // Whether or not all of this player's progress has been sent to the server
        // Reset whenever disconnecting or changing teams
        private bool _sentAllProgress = false;

        // Progress updates are queued when received from the server until you are inside of a game
        private readonly List<ProgressUpdate> progressQueue = new();

        // Save data of pers. object ids that have been interacted with
        private List<string> interactedPersistenceObjects = new ();
        public List<string> SaveInteractedObjects() => interactedPersistenceObjects;
        public void LoadInteractedObjects(List<string> objects) => interactedPersistenceObjects = objects;
        public void ClearInteractedObjects() => interactedPersistenceObjects.Clear();

        // Helper interfaces for applying / obtaining progress updates
        private readonly Dictionary<ProgressType, IProgressHelper> progressHelpers = new ()
        {
            { ProgressType.Bead, new BeadHelper() },
            { ProgressType.Prayer, new PrayerHelper() },
            { ProgressType.Relic, new RelicHelper() },
            { ProgressType.Heart, new HeartHelper() },
            { ProgressType.Collectible, new CollectibleHelper() },
            { ProgressType.QuestItem, new QuestItemHelper() },
            { ProgressType.PlayerStat, new StatHelper() },
            { ProgressType.SwordSkill, new SwordSkillHelper() },
            { ProgressType.MapCell, new MapCellHelper() },
            { ProgressType.Flag, new FlagHelper() },
            { ProgressType.PersistentObject, new PersistentObjectHelper() },
            { ProgressType.Teleport, new TeleportHelper() },
            { ProgressType.ChurchDonation, new ChurchDonationHelper() },
            { ProgressType.MiriamStatus, new MiriamHelper() },
        };

        public void ReceiveProgress(ProgressUpdate progress)
        {
            Main.Multiplayer.Log("Received new game progress: " + progress.Id);
            if (progress.ShouldSyncProgress(Main.Multiplayer.config))
            {
                progressQueue.Add(progress);
            }
        }

        public void Update()
        {
            CurrentlyUpdatingProgress = true;
            
            for (int i = 0; i < progressQueue.Count; i++)
            {
                ApplyProgress(progressQueue[i]);
            }
            progressQueue.Clear();

            CurrentlyUpdatingProgress = false;
        }

        private void ApplyProgress(ProgressUpdate progress)
        {
            if (!progressHelpers.TryGetValue(progress.Type, out IProgressHelper helper))
            {
                Main.Multiplayer.Log("Error: Progress type doesn't exist: " + progress.Type);
                return;
            }

            helper.ApplyProgress(progress);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            if (!progressHelpers.TryGetValue(progress.Type, out IProgressHelper helper))
            {
                Main.Multiplayer.Log("Error: Progress type doesn't exist: " + progress.Type);
                return null;
            }

            return helper.GetProgressNotification(progress);
        }

        // Called when interacting with pers. object - determine whether to send it or not
        public void UsePersistentObject(string persistentId)
        {
            string scene = Core.LevelManager.currentLevel.LevelName;
            int objectSceneIdx = PersistentStates.getObjectSceneIndex(scene, persistentId);
            string objectSceneId = scene + "~" + objectSceneIdx;

            // Make sure this pers. object should sync & isn't already activated
            if (objectSceneIdx < 0 || IsObjectInteracted(objectSceneId)) return;

            // Update save game data & send this object
            AddInteractedObject(objectSceneId);
            ProgressUpdate progress = new ProgressUpdate(objectSceneId, ProgressType.PersistentObject, 0);
            Main.Multiplayer.NetworkManager.SendProgress(progress);
        }

        // Called when sending all data upon connecting to server and loading game
        public void SendAllProgress()
        {
            if (_sentAllProgress) return;
            _sentAllProgress = true;

            Main.Multiplayer.DisableFileLogging = true;

            foreach (IProgressHelper helper in progressHelpers.Values)
            {
                helper.SendAllProgress();
            }

            Main.Multiplayer.DisableFileLogging = false;
        }

        public void ResetProgressSentStatus()
        {
            _sentAllProgress = false;
            progressQueue.Clear();
        }

        public void LevelLoaded(string scene)
        {
            foreach (PersistentObject persistence in Object.FindObjectsOfType<PersistentObject>())
            {
                int objectSceneIdx = PersistentStates.getObjectSceneIndex(scene, persistence.GetPersistenID());
                string objectSceneId = scene + "~" + objectSceneIdx;

                // This object does not even sync or hasn't been interacted with yet
                if (objectSceneIdx < 0 || !IsObjectInteracted(objectSceneId)) continue;

                // Calling setPersistence() with null data means to play instant animation
                persistence.SetCurrentPersistentState(null, false, null);
            }
        }

        // Checks whether or not a persistent object has been interacted with
        public bool IsObjectInteracted(string objectSceneId)
        {
            return interactedPersistenceObjects.Contains(objectSceneId);
        }

        public void AddInteractedObject(string objectSceneId)
        {
            interactedPersistenceObjects.Add(objectSceneId);
        }

        public List<string> GetAllInteractedObjects()
        {
            return interactedPersistenceObjects;
        }
    }
}
