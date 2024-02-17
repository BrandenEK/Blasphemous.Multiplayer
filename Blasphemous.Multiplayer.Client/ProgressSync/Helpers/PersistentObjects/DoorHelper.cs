using Framework.Managers;
using HarmonyLib;
using Tools.Level.Interactables;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(Door), "EnterDoor")]
    public class DoorUse_Patch
    {
        public static void Postfix(Door __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Door opened: " + persistentId);
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }

    [HarmonyPatch(typeof(Door), "GetCurrentPersistentState")]
    public class DoorReceive_Patch
    {
        public static bool Prefix(string dataPath, Door __instance, ref bool ___objectUsed)
        {
            if (dataPath != "use") return true;

            ___objectUsed = true;
            __instance.Closed = false; // Play anim ?
            return false;
        }
    }

    [HarmonyPatch(typeof(Door), "SetCurrentPersistentState")]
    public class DoorLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, Door __instance, Animator ___interactableAnimator, ref bool ___objectUsed)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            ___objectUsed = true;
            __instance.Closed = false;
            ___interactableAnimator.SetTrigger("INSTA_OPEN");
            return false;
        }
    }
}
