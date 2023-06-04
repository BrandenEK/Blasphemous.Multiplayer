using BlasClient.Data;
using Framework.Managers;
using HarmonyLib;
using System.Collections.Generic;

namespace BlasClient.ProgressSync.Helpers
{
    public class FlagHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            if (!Main.Multiplayer.config.syncSettings.worldState) return;

            Core.Events.SetFlag(progress.Id, progress.Value == 0, false);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            FlagState flag = FlagStates.getFlagState(progress.Id);
            return flag?.notification;
        }

        public void SendAllProgress()
        {
            Core.Events.GetCurrentPersistentState("intro", false);
        }
    }

    [HarmonyPatch(typeof(EventManager), "SetFlag")]
    public class EventManager_Patch
    {
        public static void Postfix(EventManager __instance, string id, bool b)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress || !Main.Multiplayer.config.syncSettings.worldState)
                return;

            string formatted = __instance.GetFormattedId(id);
            if (FlagStates.getFlagState(formatted) != null)
            {
                ProgressUpdate progress = new ProgressUpdate(formatted, ProgressType.Flag, (byte)(b ? 0 : 1));
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    // This also handles the miriam status section
    [HarmonyPatch(typeof(EventManager), "GetCurrentPersistentState")]
    public class EventManagerIntro_Patch
    {
        public static bool Prefix(string dataPath, Dictionary<string, FlagObject> ___flags, List<string> ___MiriamClosedPortals)
        {
            // Calling this with 'intro' means it should send all set flags
            if (dataPath != "intro") return true;

            foreach (string flag in ___flags.Keys)
            {
                if (FlagStates.getFlagState(flag) != null && ___flags[flag].value)
                {
                    ProgressUpdate progress = new ProgressUpdate(flag, ProgressType.Flag, 0);
                    Main.Multiplayer.NetworkManager.SendProgress(progress);
                }
            }
            foreach (string portal in ___MiriamClosedPortals)
            {
                ProgressUpdate progress = new ProgressUpdate(portal, ProgressType.MiriamStatus, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
            return false;
        }
    }
}
