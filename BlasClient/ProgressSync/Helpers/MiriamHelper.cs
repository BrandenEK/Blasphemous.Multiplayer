using Framework.Managers;
using HarmonyLib;
using System.Collections.Generic;

namespace BlasClient.ProgressSync.Helpers
{
    public class MiriamHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            string status = progress.Id;
            if (status == "START")
                Core.Events.StartMiriamQuest();
            else if (status == "FINISH")
                Core.Events.FinishMiriamQuest();
            else if (status == "D02Z03S24" || status == "D03Z03S19" || status == "D04Z04S02" || status == "D05Z01S24" || status == "D06Z01S26")
                Core.Events.SetCurrentPersistentState(null, false, status);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            return null;
        }

        public void SendAllProgress()
        {
            if (Core.Events.IsMiriamQuestStarted)
            {
                ProgressUpdate progress = new ProgressUpdate("START", ProgressType.MiriamStatus, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
            if (Core.Events.IsMiriamQuestFinished)
            {
                ProgressUpdate progress = new ProgressUpdate("FINISH", ProgressType.MiriamStatus, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
            // The miriam closed portals are handled in the flag helper
        }
    }

    // Send miriam closed portal
    [HarmonyPatch(typeof(EventManager), "EndMiriamPortalAndReturn")]
    public class EventManagerMiriamPortal_Patch
    {
        public static void Prefix(EventManager __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
                return;

            if (__instance.AreInMiriamLevel())
            {
                ProgressUpdate progress = new ProgressUpdate(__instance.MiriamCurrentScenePortal, ProgressType.MiriamStatus, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    // Send miriam quest start
    [HarmonyPatch(typeof(EventManager), "StartMiriamQuest")]
    public class EventManagerMiriamStart_Patch
    {
        public static void Postfix(bool __result)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
                return;

            if (__result)
            {
                ProgressUpdate progress = new ProgressUpdate("START", ProgressType.MiriamStatus, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    // Send miriam quest finish
    [HarmonyPatch(typeof(EventManager), "FinishMiriamQuest")]
    public class EventManagerMiriamFinish_Patch
    {
        public static void Postfix(bool __result)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
                return;

            if (__result)
            {
                ProgressUpdate progress = new ProgressUpdate("FINISH", ProgressType.MiriamStatus, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
        }
    }

    // Receive closed portal
    [HarmonyPatch(typeof(EventManager), "SetCurrentPersistentState")]
    public class EventManagerMiriamReceive_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, string dataPath, List<string> ___MiriamClosedPortals)
        {
            if (data == null)
            {
                // Called specially - close specified portal
                if (!___MiriamClosedPortals.Contains(dataPath))
                    ___MiriamClosedPortals.Add(dataPath);
                return false;
            }
            else
            {
                // Called normally
                return true;
            }
        }
    }
}
