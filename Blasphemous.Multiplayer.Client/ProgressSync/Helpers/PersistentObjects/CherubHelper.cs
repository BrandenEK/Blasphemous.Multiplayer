using Framework.Managers;
using HarmonyLib;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(CherubCaptorPersistentObject), "OnCherubKilled")]
    public class CherubUse_Patch
    {
        public static void Postfix(CherubCaptorPersistentObject __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Cherub killed: " + persistentId);
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }
    [HarmonyPatch(typeof(CherubCaptorPersistentObject), "GetCurrentPersistentState")]
    public class CherubReceive_Patch
    {
        public static bool Prefix(string dataPath, CherubCaptorPersistentObject __instance)
        {
            if (dataPath != "use") return true;

            // Play animation death
            __instance.destroyed = true;
            __instance.spawner.DisableCherubSpawn();
            __instance.spawner.DestroySpawnedCherub();
            return false;
        }
    }
    [HarmonyPatch(typeof(CherubCaptorPersistentObject), "SetCurrentPersistentState")]
    public class CherubLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, CherubCaptorPersistentObject __instance)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            __instance.destroyed = true;
            __instance.spawner.DisableCherubSpawn();
            __instance.spawner.DestroySpawnedCherub();
            return false;
        }
    }
}
