using System.Collections.Generic;
using HarmonyLib;
using Framework.Managers;
using Gameplay.UI.Widgets;
using Gameplay.UI.Console;
using Gameplay.UI.Others;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    // Allow access to console
    [HarmonyPatch(typeof(ConsoleWidget), "Update")]
    public class ConsoleWidget_Patch
    {
        public static void Postfix(ConsoleWidget __instance, bool ___isEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Backslash))
            {
                __instance.SetEnabled(!___isEnabled);
            }
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
    // Allow console commands on the main menu
    [HarmonyPatch(typeof(KeepFocus), "Update")]
    public class KeepFocus_Patch
    {
        public static bool Prefix()
        {
            return ConsoleWidget.Instance == null || !ConsoleWidget.Instance.IsEnabled();
        }
    }
    [HarmonyPatch(typeof(ConsoleWidget), "SetEnabled")]
    public class ConsoleWidgetDisable_Patch
    {
        public static void Postfix(bool enabled)
        {
            if (!enabled && Core.LevelManager.currentLevel.LevelName == "MainMenu")
            {
                Button[] buttons = Object.FindObjectsOfType<Button>();
                foreach (Button b in buttons)
                {
                    if (b.name == "Continue")
                    {
                        EventSystem.current.SetSelectedGameObject(b.gameObject);
                        return;
                    }
                }
            }
        }
    }
}
