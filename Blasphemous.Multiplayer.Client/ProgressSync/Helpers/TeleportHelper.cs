using Framework.FrameworkCore;
using Framework.Managers;
using HarmonyLib;
using System.Collections.Generic;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers
{
    public class TeleportHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Core.SpawnManager.SetTeleportActive(progress.Id, true);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            return null;
        }

        public void SendAllProgress()
        {
            Core.SpawnManager.GetCurrentPersistentState("intro", false);
        }
    }

    [HarmonyPatch(typeof(SpawnManager), "SetTeleportActive")]
    class SpawnManager_Patch
    {
        public static void Postfix(string teleportId, bool active)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
                return;

            if (active)
            {
                ProgressUpdate progress = new ProgressUpdate(teleportId, ProgressType.Teleport, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    [HarmonyPatch(typeof(SpawnManager), "GetCurrentPersistentState")]
    class SpawnManagerIntro_Patch
    {
        public static bool Prefix(string dataPath, Dictionary<string, TeleportDestination> ___TeleportDict)
        {
            // Calling this with 'intro' means it should send all unlocked teleports
            if (dataPath != "intro") return true;

            foreach (string key in ___TeleportDict.Keys)
            {
                if (___TeleportDict[key].isActive)
                {
                    ProgressUpdate progress = new ProgressUpdate(key, ProgressType.Teleport, 0);
                    Main.Multiplayer.NetworkManager.SendProgress(progress);
                }
            }
            return false;
        }
    }
}
