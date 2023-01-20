using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Framework.Managers;

namespace BlasClient
{
    // Initialize Multiplayer class
    [HarmonyPatch(typeof(AchievementsManager), "AllInitialized")]
    public class AchievementsManager_InitializePatch
    {
        public static void Postfix()
        {
            Main.Multiplayer.Initialize();
        }
    }
    // Dispose Multiplayer class
    [HarmonyPatch(typeof(AchievementsManager), "Dispose")]
    public class AchievementsManager_DisposePatch
    {
        public static void Postfix()
        {
            Main.Multiplayer.Dispose();
        }
    }
}
