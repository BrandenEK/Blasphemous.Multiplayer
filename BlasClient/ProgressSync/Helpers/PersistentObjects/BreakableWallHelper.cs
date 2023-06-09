using Framework.Managers;
using HarmonyLib;
using Tools.Level.Actionables;

namespace BlasClient.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(BreakableWall), "Damage")]
    public class BreakableWallUse_Patch
    {
        public static void Postfix(BreakableWall __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Broke wall: " + persistentId);
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }

    [HarmonyPatch(typeof(BreakableWall), "GetCurrentPersistentState")]
    public class BreakableWallReceive_Patch
    {
        public static bool Prefix(string dataPath, BreakableWall __instance)
        {
            if (dataPath != "use") return true;

            __instance.Use(); // Needs to be changed
            return false;
        }
    }

    [HarmonyPatch(typeof(BreakableWall), "SetCurrentPersistentState")]
    public class BreakableWallLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, BreakableWall __instance)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            __instance.Use(); // Needs to be changed
            return false;
        }
    }
}
