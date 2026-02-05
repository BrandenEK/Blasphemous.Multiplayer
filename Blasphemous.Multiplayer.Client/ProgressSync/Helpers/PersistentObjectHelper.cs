using Blasphemous.ModdingAPI;
using Blasphemous.Multiplayer.Client.Data;
using Blasphemous.Multiplayer.Common.Enums;
using Framework.FrameworkCore;
using Framework.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers
{
    public class PersistentObjectHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Main.Multiplayer.ProgressManager.AddInteractedObject(progress.Id);
            string[] sections = progress.Id.Split('~');
            string objectScene = sections[0];
            int objectSceneIdx = int.Parse(sections[1]);
            string objectPersistentId = PersistentStates.getObjectPersistentId(objectScene, objectSceneIdx);

            if (Core.LevelManager.currentLevel.LevelName != objectScene || objectPersistentId == null)
                return;

            // Player just received a pers. object in the same scene - find it and set value immediately
            foreach (PersistentObject persistentObject in Object.FindObjectsOfType<PersistentObject>())
            {
                try
                {
                    if (persistentObject.GetPersistenID() == objectPersistentId)
                    {
                        // Calling getPersistence() with "use" means to play used animation
                        persistentObject.GetCurrentPersistentState("use", false);
                        break;
                    }
                }
                catch (System.NullReferenceException)
                {
                    ModLog.Error("Error: Failed to get persistent id of object");
                }
            }
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            return null;
        }

        public void SendAllProgress()
        {
            List<string> objects = Main.Multiplayer.ProgressManager.GetAllInteractedObjects();
            for (int i = 0; i < objects.Count; i++)
            {
                ProgressUpdate progress = new ProgressUpdate(objects[i], ProgressType.PersistentObject, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }
}
