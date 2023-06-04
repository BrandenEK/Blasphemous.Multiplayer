using Framework.Managers;
using HarmonyLib;
using Tools.Level.Interactables;
using UnityEngine;

namespace BlasClient.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(Chest), "OnUse")]
    public class ChestUse_Patch
    {
        public static void Postfix(Chest __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log($"Used Chest: {persistentId}");
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }

    [HarmonyPatch(typeof(Chest), "GetCurrentPersistentState")]
    public class ChestReceive_Patch
    {
        public static bool Prefix(string dataPath, Chest __instance, Animator ___interactableAnimator)
        {
            if (dataPath != "use") return true;

            __instance.Consumed = true;
            ___interactableAnimator.SetBool("USED", true);
            return false;
        }
    }

    [HarmonyPatch(typeof(Chest), "SetCurrentPersistentState")]
    public class ChestLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, Chest __instance, Animator ___interactableAnimator)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            __instance.Consumed = true;
            ___interactableAnimator.SetBool("NOANIMUSED", true);
            return false;
        }
    }
}
