using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Framework.Managers;
using Framework.Inventory;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes;
using Framework.FrameworkCore.Attributes.Logic;
using Framework.Map;
using Tools.Level;
using Tools.Level.Actionables;
using Tools.Level.Interactables;
using Gameplay.GameControllers.Environment.MovingPlatforms;
using BlasClient.Managers;

namespace BlasClient.Patches
{
    // Inventory items

    [HarmonyPatch(typeof(InventoryManager), "AddRosaryBead", typeof(RosaryBead))] // TODO - Change this to use the AddBaseObject method
    public class InventoryBead_Patch
    {
        public static void Postfix(RosaryBead rosaryBead)
        {
            if (!ProgressManager.updatingProgress)
            {
                Main.Multiplayer.obtainedGameProgress(rosaryBead.id, 0, 0);
            }
        }
    }
    [HarmonyPatch(typeof(InventoryManager), "AddPrayer", typeof(Prayer))]
    public class InventoryPrayer_Patch
    {
        public static void Postfix(Prayer prayer)
        {
            if (!ProgressManager.updatingProgress)
            {
                Main.Multiplayer.obtainedGameProgress(prayer.id, 1, 0);
            }
        }
    }
    [HarmonyPatch(typeof(InventoryManager), "AddRelic", typeof(Relic))]
    public class InventoryRelic_Patch
    {
        public static void Postfix(Relic relic)
        {
            if (!ProgressManager.updatingProgress)
            {
                Main.Multiplayer.obtainedGameProgress(relic.id, 2, 0);
            }
        }
    }
    [HarmonyPatch(typeof(InventoryManager), "AddSword", typeof(Sword))]
    public class InventorySword_Patch
    {
        public static void Postfix(Sword sword)
        {
            if (!ProgressManager.updatingProgress)
            {
                Main.Multiplayer.obtainedGameProgress(sword.id, 3, 0);
            }
        }
    }
    [HarmonyPatch(typeof(InventoryManager), "AddCollectibleItem", typeof(Framework.Inventory.CollectibleItem))]
    public class InventoryCollectible_Patch
    {
        public static void Postfix(Framework.Inventory.CollectibleItem collectibleItem)
        {
            if (!ProgressManager.updatingProgress)
            {
                Main.Multiplayer.obtainedGameProgress(collectibleItem.id, 4, 0);
            }
        }
    }
    [HarmonyPatch(typeof(InventoryManager), "AddQuestItem", typeof(QuestItem))]
    public class InventoryQuestItem_Patch
    {
        public static void Postfix(QuestItem questItem)
        {
            if (!ProgressManager.updatingProgress)
            {
                Main.Multiplayer.obtainedGameProgress(questItem.id, 5, 0);
            }
        }
    }

    // Player stats

    [HarmonyPatch(typeof(Attribute), "Upgrade")]
    public class LifeUpgrade_Patch
    {
        public static void Postfix(Attribute __instance)
        {
            if (ProgressManager.updatingProgress)
                return;

            byte type = 255;
            if (__instance.GetType() == typeof(Life)) type = 6;
            else if (__instance.GetType() == typeof(Fervour)) type = 7;
            else if (__instance.GetType() == typeof(Strength)) type = 8;
            else if (__instance.GetType() == typeof(MeaCulpa)) type = 9;
            else if (__instance.GetType() == typeof(BeadSlots)) type = 10;
            else if (__instance.GetType() == typeof(Flask)) type = 11;
            else if (__instance.GetType() == typeof(FlaskHealth)) type = 12;

            if (type != 255)
                Main.Multiplayer.obtainedGameProgress("Stat", type, 0);
        }
    }

    // Sword skills

    [HarmonyPatch(typeof(SkillManager), "UnlockSkill")]
    public class SkilManager_Patch
    {
        public static bool Prefix(string skill, Dictionary<string, UnlockableSkill> ___allSkills)
        {
            if (ProgressManager.updatingProgress)
            {
                // Just received this skill from another player, skip checks & cost
                ___allSkills[skill].unlocked = true;
                return false;
            }
            else
            {
                // Actually obtaining item, send to other players
                Main.Multiplayer.obtainedGameProgress(skill, 13, 0);
                return true;
            }
        }
    }

    // Game flags

    [HarmonyPatch(typeof(EventManager), "SetFlag")]
    public class EventManager_Patch
    {
        public static void Postfix(EventManager __instance, string id, bool b)
        {
            string formatted = __instance.GetFormattedId(id);
            if (!ProgressManager.updatingProgress && StaticObjects.getFlagState(formatted) != null)
            {
                Main.Multiplayer.obtainedGameProgress(formatted, 14, (byte)(b ? 0 : 1));
            }
        }
    }

    // Warp teleports

    [HarmonyPatch(typeof(SpawnManager), "SetTeleportActive")]
    public class SpawnManager_Patch
    {
        public static void Postfix(string teleportId)
        {
            Main.UnityLog("Unlocked teleport");
            if (!ProgressManager.updatingProgress)
            {
                Main.Multiplayer.obtainedGameProgress(teleportId, 16, 0);
            }
        }
    }

    // Map cells

    [HarmonyPatch(typeof(NewMapManager), "RevealCellInPosition")]
    public class NewMapManager_Patch
    {
        public static bool Prefix(NewMapManager __instance, Vector2 position, MapData ___CurrentMap)
        {
            if (!ProgressManager.updatingProgress)
            {
                // Actually revealing a new cell
                if (___CurrentMap == null || !___CurrentMap.CellsByZone.ContainsKey(__instance.CurrentScene))
                    return false;

                foreach (CellData cell in ___CurrentMap.CellsByZone[__instance.CurrentScene])
                {
                    if (cell.Bounding.Contains(position) && !cell.Revealed)
                        Main.Multiplayer.obtainedGameProgress(___CurrentMap.Cells.IndexOf(cell).ToString(), 17, 0);
                }
                return true;
            }
            else
            {
                // Received this new cell from other player, skip other stuff
                int cellIdx = Mathf.RoundToInt(position.x);
                if (___CurrentMap != null && cellIdx >= 0 && cellIdx < ___CurrentMap.Cells.Count)
                {
                    ___CurrentMap.Cells[cellIdx].Revealed = true;
                }
                return false;
            }
        }
    }

    // Persistent objects

    // Interactable use (PrieDieus, CollectibleItems, Chests, Levers)
    [HarmonyPatch(typeof(Interactable), "Use")] // Change to patches for each type of pers. object
    public class InteractableUse_Patch
    {
        public static void Postfix(Interactable __instance)
        {
            string persistentId = __instance.GetPersistenID();
            Main.UnityLog("Using object: " + persistentId + ", type: " + __instance.GetType().ToString()); // temp
            if (!ProgressManager.updatingProgress && StaticObjects.GetPersistenceState(persistentId) != null && !Main.Multiplayer.checkPersistentObject(persistentId))
            {
                // Update save game data & send this object
                Main.Multiplayer.addPersistentObject(persistentId);
                Main.Multiplayer.obtainedGameProgress(persistentId, 15, 0);
            }
        }
    }

    // Cherub use
    [HarmonyPatch(typeof(CherubCaptorPersistentObject), "OnCherubKilled")]
    public class CherubCaptorUse_Patch
    {
        public static void Postfix(CherubCaptorPersistentObject __instance)
        {
            string persistentId = __instance.GetPersistenID();
            Main.UnityLog("Cherub killed: " + persistentId);
            if (!ProgressManager.updatingProgress && StaticObjects.GetPersistenceState(persistentId) != null && !Main.Multiplayer.checkPersistentObject(persistentId))
            {
                // Update save game data & send this object
                Main.Multiplayer.addPersistentObject(persistentId);
                Main.Multiplayer.obtainedGameProgress(persistentId, 15, 0);
            }
        }
    }

    // Gate use
    [HarmonyPatch(typeof(Gate), "Use")]
    public class Gate_Patch
    {
        public static void Postfix(Gate __instance)
        {
            string persistentId = __instance.GetPersistenID();
            Main.UnityLog("gate opened: " + persistentId);
            if (!ProgressManager.updatingProgress && StaticObjects.GetPersistenceState(persistentId) != null && !Main.Multiplayer.checkPersistentObject(persistentId))
            {
                // Update save game data & send this object
                Main.Multiplayer.addPersistentObject(persistentId);
                Main.Multiplayer.obtainedGameProgress(persistentId, 15, 0);
            }
        }
    }

    // Moving platform use
    [HarmonyPatch(typeof(StraightMovingPlatform), "Use")]
    public class MovingPlatformUse_Patch
    {
        public static void Postfix(StraightMovingPlatform __instance)
        {
            string persistentId = __instance.GetPersistenID();
            Main.UnityLog("Activated platform: " + persistentId);
            if (!ProgressManager.updatingProgress && StaticObjects.GetPersistenceState(persistentId) != null && !Main.Multiplayer.checkPersistentObject(persistentId))
            {
                // Update save game data & send this object
                Main.Multiplayer.addPersistentObject(persistentId);
                Main.Multiplayer.obtainedGameProgress(persistentId, 15, 0);
            }
        }
    }

    // Slash trigger use
    [HarmonyPatch(typeof(TriggerReceiver), "Use")]
    public class SlashTriggerUse_Patch
    {
        public static void Postfix(TriggerReceiver __instance)
        {
            string persistentId = __instance.GetPersistenID();
            Main.UnityLog("Trigger activated: " + persistentId);
            if (!ProgressManager.updatingProgress && StaticObjects.GetPersistenceState(persistentId) != null && !Main.Multiplayer.checkPersistentObject(persistentId))
            {
                // Update save game data & send this object
                Main.Multiplayer.addPersistentObject(persistentId);
                Main.Multiplayer.obtainedGameProgress(persistentId, 15, 0);
            }
        }
    }

    // Breakable walls
    [HarmonyPatch(typeof(BreakableWall), "Damage")]
    public class BreakableWall_Patch
    {
        public static void Postfix(BreakableWall __instance)
        {
            Main.UnityLog("Broke wall: " + __instance.GetPersistenID());
        }
    }

    // PrieDieu load
    [HarmonyPatch(typeof(PrieDieu), "SetCurrentPersistentState")]
    public class PrieDieuLoad_Patch
    {
        public static bool Prefix(PrieDieu __instance, PersistentManager.PersistentData data)
        {
            if (data == null)
            {
                __instance.Ligthed = true;
                return false;
            }
            return true;
        }
    }

    // Collectible item load
    [HarmonyPatch(typeof(CollectibleItem), "SetCurrentPersistentState")]
    public class CollectibleItemLoad_Patch
    {
        public static bool Prefix(CollectibleItem __instance, Animator ___interactableAnimator, PersistentManager.PersistentData data)
        {
            if (data == null)
            {
                __instance.Consumed = true;
                ___interactableAnimator.gameObject.SetActive(false);
                return false;
            }
            return true;
        }
    }

    // Chest load
    [HarmonyPatch(typeof(Chest), "SetCurrentPersistentState")]
    public class ChestLoad_Patch
    {
        public static bool Prefix(Chest __instance, Animator ___interactableAnimator, PersistentManager.PersistentData data)
        {
            if (data == null)
            {
                __instance.Consumed = true;
                ___interactableAnimator.SetBool("NOANIMUSED", true);
                return false;
            }
            return true;
        }
    }

    // Cherub load
    [HarmonyPatch(typeof(CherubCaptorPersistentObject), "SetCurrentPersistentState")]
    public class CherubLoad_Patch
    {
        public static bool Prefix(CherubCaptorPersistentObject __instance, PersistentManager.PersistentData data)
        {
            if (data == null)
            {
                __instance.destroyed = true;
                __instance.spawner.DisableCherubSpawn();
                __instance.spawner.DestroySpawnedCherub();
                return false;
            }
            return true;
        }
    }

    // Lever load
    [HarmonyPatch(typeof(Lever), "SetCurrentPersistentState")]
    public class LeverLoad_Patch
    {
        public static bool Prefix(Lever __instance, PersistentManager.PersistentData data)
        {
            if (data == null)
            {
                __instance.Consumed = true;
                __instance.SetLeverDownInstantly();
                return false;
            }
            return true;
        }
    }

    // Gate load
    [HarmonyPatch(typeof(Gate), "SetCurrentPersistentState")]
    public class GateLoad_Patch
    {
        public static bool Prefix(ref bool ___open, ref Animator ___animator, ref Collider2D ___collision, bool ___persistState, PersistentManager.PersistentData data)
        {
            if (data == null && ___persistState)
            {
                ___open = true;
                ___collision.enabled = false;
                ___animator.SetBool("INSTA_ACTION", true);
                ___animator.SetBool("OPEN", true);
                return false;
            }
            return true;
        }
    }

    // Moving platform load
    [HarmonyPatch(typeof(StraightMovingPlatform), "SetCurrentPersistentState")]
    public class MovingPlatformLoad_Patch
    {
        public static bool Prefix(bool ___persistState, ref bool ____running, string ___OnDestination, PersistentManager.PersistentData data)
        {
            if (data == null && ___persistState)
            {
                ____running = !Core.Events.GetFlag(___OnDestination);
                return false;
            }
            return true;
        }
    }

    // Slash trigger load
    [HarmonyPatch(typeof(TriggerReceiver), "SetCurrentPersistentState")]
    public class SlashTriggerLoad_Patch
    {
        public static bool Prefix(TriggerReceiver __instance, ref bool ___alreadyUsed, PersistentManager.PersistentData data)
        {
            if (data == null)
            {
                ___alreadyUsed = true;
                __instance.animator.Play("USED");
                Collider2D collider = __instance.GetComponent<Collider2D>();
                if (collider != null) collider.enabled = false;
                return false;
            }
            return true;
        }
    }

    // Temporarily allow teleportation
    [HarmonyPatch(typeof(AlmsManager), "GetPrieDieuLevel")]
    public class AlmsManager_Patch
    {
        public static bool Prefix(ref int __result)
        {
            __result = 3;
            return false;
        }
    }
}
