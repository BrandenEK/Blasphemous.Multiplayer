﻿using Blasphemous.ModdingAPI;
using Framework.Managers;
using HarmonyLib;
using Tools.Level.Actionables;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.ProgressSync.Helpers.PersistentObjects
{
    [HarmonyPatch(typeof(Gate), "Use")]
    class GateUse_Patch
    {
        public static void Postfix(Gate __instance)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            ModLog.Info("Gate opened: " + persistentId);
            Main.Multiplayer.ProgressManager.UsePersistentObject(persistentId);
        }
    }
    [HarmonyPatch(typeof(Gate), "GetCurrentPersistentState")]
    class GateReceive_Patch
    {
        public static bool Prefix(string dataPath, ref bool ___open, Animator ___animator, Collider2D ___collision)
        {
            if (dataPath != "use") return true;

            ___open = true;
            ___collision.enabled = false;
            ___animator.SetBool("INSTA_ACTION", false);
            ___animator.SetBool("OPEN", true);
            return false;
        }
    }
    [HarmonyPatch(typeof(Gate), "SetCurrentPersistentState")]
    class GateLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, Gate __instance, ref bool ___open, Animator ___animator, Collider2D ___collision)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.ProgressManager.IsObjectInteracted(__instance.GetPersistenID());
            }

            ___open = true;
            ___collision.enabled = false;
            ___animator.SetBool("INSTA_ACTION", true);
            ___animator.SetBool("OPEN", true);
            return false;
        }
    }
}
