using Blasphemous.ModdingAPI;
using Framework.Managers;
using HarmonyLib;
using Tools.Level.Actionables;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(ActionableLadder), "Use")]
    class LadderUse_Patch
    {
        public static void Postfix(ActionableLadder __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            ModLog.Info("Ladder activated: " + persistentId);
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }

    [HarmonyPatch(typeof(ActionableLadder), "GetCurrentPersistentState")]
    class LadderReceive_Patch
    {
        public static bool Prefix(string dataPath, ActionableLadder __instance, ref bool ___open)
        {
            if (dataPath != "use") return true;

            ___open = false;
            __instance.Use(); // Needs to be changed
            return false;
        }
    }

    [HarmonyPatch(typeof(ActionableLadder), "SetCurrentPersistentState")]
    class LadderLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, ActionableLadder __instance, ref bool ___open)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            ___open = false;
            __instance.Use(); // Needs to be changed
            return false;
        }
    }
}
