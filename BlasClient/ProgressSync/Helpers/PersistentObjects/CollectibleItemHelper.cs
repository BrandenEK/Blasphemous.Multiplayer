using Framework.Managers;
using HarmonyLib;
using UnityEngine;

namespace BlasClient.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(CollectibleItem), "OnUse")]
    public class CollectibleItemUse_Patch
    {
        public static void Postfix(CollectibleItem __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log($"Used CollectibleItem: {persistentId}");
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }

    [HarmonyPatch(typeof(CollectibleItem), "GetCurrentPersistentState")]
    public class CollectibleItemReceive_Patch
    {
        public static bool Prefix(string dataPath, CollectibleItem __instance, Animator ___interactableAnimator)
        {
            if (dataPath != "use") return true;

            __instance.Consumed = true;
            ___interactableAnimator.gameObject.SetActive(false);
            return false;
        }
    }

    [HarmonyPatch(typeof(CollectibleItem), "SetCurrentPersistentState")]
    public class CollectibleItemLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, CollectibleItem __instance, Animator ___interactableAnimator)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            __instance.Consumed = true;
            ___interactableAnimator.gameObject.SetActive(false);
            return false;
        }
    }
}
