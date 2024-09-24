using Blasphemous.ModdingAPI;
using Framework.Managers;
using HarmonyLib;
using Tools.Level.Actionables;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(TriggerReceiver), "Use")]
    class TriggerReceiverUse_Patch
    {
        public static void Postfix(TriggerReceiver __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            ModLog.Info("Trigger activated: " + persistentId);
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }
    [HarmonyPatch(typeof(TriggerReceiver), "GetCurrentPersistentState")]
    class TriggerReceiverReceive_Patch
    {
        public static bool Prefix(string dataPath, TriggerReceiver __instance, ref bool ___alreadyUsed)
        {
            if (dataPath != "use") return true;

            ___alreadyUsed = true;
            __instance.animator.SetTrigger("ACTIVATE");
            Collider2D collider = __instance.GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(TriggerReceiver), "SetCurrentPersistentState")]
    class TriggerReceiverLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, TriggerReceiver __instance, ref bool ___alreadyUsed)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            ___alreadyUsed = true;
            __instance.animator.Play("USED");
            Collider2D collider = __instance.GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;
            return false;
        }
    }
}
