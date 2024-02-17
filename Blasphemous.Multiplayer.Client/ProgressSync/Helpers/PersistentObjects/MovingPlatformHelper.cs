using Blasphemous.Multiplayer.Client.Data;
using Framework.Managers;
using HarmonyLib;
using Gameplay.GameControllers.Environment.MovingPlatforms;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(StraightMovingPlatform), "Use")]
    public class MovingPlatformUse_Patch
    {
        public static void Postfix(StraightMovingPlatform __instance, ref bool ____running)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Activated platform: " + persistentId);
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);

            // Make sure this platform is set to running when activated twice
            if (PersistentStates.getObjectSceneIndex(Core.LevelManager.currentLevel.LevelName, persistentId) >= 0)
            {
                ____running = true;
            }
        }
    }

    [HarmonyPatch(typeof(StraightMovingPlatform), "GetCurrentPersistentState")]
    public class MovingPlatformReceive_Patch
    {
        public static bool Prefix(string dataPath, StraightMovingPlatform __instance)
        {
            if (dataPath != "use") return true;

            __instance.Reset();
            __instance.Use();
            return false;
        }
    }

    [HarmonyPatch(typeof(StraightMovingPlatform), "SetCurrentPersistentState")]
    public class MovingPlatformLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, StraightMovingPlatform __instance)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            __instance.Reset();
            __instance.Use();
            return false;
        }
    }
}
