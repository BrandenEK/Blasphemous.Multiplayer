using HarmonyLib;
using System.Collections.Generic;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Gameplay.UI.Console;

namespace BlasClient.Patches
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

    // Add multiplayer commands to console
    [HarmonyPatch(typeof(ConsoleWidget), "InitializeCommands")]
    public class Console_Patch
    {
        public static void Postfix(List<ConsoleCommand> ___commands)
        {
            ___commands.Add(new MultiplayerCommand());
        }
    }
    
    // Send updated skin when picking a new one
    [HarmonyPatch(typeof(ColorPaletteManager), "SetCurrentColorPaletteId")]
    public class ColorPaletteManager_Patch
    {
        public static void Postfix(string colorPaletteId)
        {
            Main.Multiplayer.changeSkin(colorPaletteId);
        }
    }
}
