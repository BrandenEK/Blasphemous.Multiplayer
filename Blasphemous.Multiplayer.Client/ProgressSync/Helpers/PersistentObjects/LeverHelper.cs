using Blasphemous.ModdingAPI;
using Framework.Managers;
using HarmonyLib;
using Tools.Level.Interactables;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(Lever), "OnUse")]
    class LeverUse_Patch
    {
        public static void Postfix(Lever __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            ModLog.Info($"Used Lever: {persistentId}");
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }

    [HarmonyPatch(typeof(Lever), "GetCurrentPersistentState")]
    class LeverReceive_Patch
    {
        public static bool Prefix(string dataPath, Lever __instance, Animator ___interactableAnimator)
        {
            if (dataPath != "use") return true;

            __instance.Consumed = true;
            ___interactableAnimator.SetBool("ACTIVE", true); // Might still activate other objects
            return false;
        }
    }

    [HarmonyPatch(typeof(Lever), "SetCurrentPersistentState")]
    class LeverLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, Lever __instance)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            __instance.Consumed = true;
            __instance.SetLeverDownInstantly();
            return false;
        }
    }
}
