﻿using HarmonyLib;
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
using BlasClient.Data;

namespace BlasClient.Patches
{
    // Inventory items

    // Beads
    [HarmonyPatch(typeof(InventoryManager), "AddRosaryBead", typeof(RosaryBead))]
    public class InventoryBead_Patch
    {
        public static void Postfix(RosaryBead rosaryBead)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.inventoryItems)
            {
                Main.Multiplayer.obtainedGameProgress(rosaryBead.id, ProgressManager.ProgressType.Bead, 0);
            }
        }
    }
    [HarmonyPatch(typeof(InventoryManager), "RemoveRosaryBead", typeof(RosaryBead))]
    public class InventoryBeadRemove_Patch
    {
        public static void Postfix(RosaryBead bead)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.inventoryItems)
            {
                Main.Multiplayer.obtainedGameProgress(bead.id, ProgressManager.ProgressType.Bead, 1);
            }
        }
    }

    // Prayers
    [HarmonyPatch(typeof(InventoryManager), "AddPrayer", typeof(Prayer))]
    public class InventoryPrayer_Patch
    {
        public static void Postfix(Prayer prayer)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.inventoryItems)
            {
                Main.Multiplayer.obtainedGameProgress(prayer.id, ProgressManager.ProgressType.Prayer, 0);
            }
        }
    }

    // Relics
    [HarmonyPatch(typeof(InventoryManager), "AddRelic", typeof(Relic))]
    public class InventoryRelic_Patch
    {
        public static void Postfix(Relic relic)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.inventoryItems)
            {
                Main.Multiplayer.obtainedGameProgress(relic.id, ProgressManager.ProgressType.Relic, 0);
            }
        }
    }

    // Hearts
    [HarmonyPatch(typeof(InventoryManager), "AddSword", typeof(Sword))]
    public class InventorySword_Patch
    {
        public static void Postfix(Sword sword)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.inventoryItems)
            {
                Main.Multiplayer.obtainedGameProgress(sword.id, ProgressManager.ProgressType.Heart, 0);
            }
        }
    }

    // Collectibles
    [HarmonyPatch(typeof(InventoryManager), "AddCollectibleItem", typeof(Framework.Inventory.CollectibleItem))]
    public class InventoryCollectible_Patch
    {
        public static void Postfix(Framework.Inventory.CollectibleItem collectibleItem)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.inventoryItems)
            {
                Main.Multiplayer.obtainedGameProgress(collectibleItem.id, ProgressManager.ProgressType.Collectible, 0);
            }
        }
    }

    // Quest items
    [HarmonyPatch(typeof(InventoryManager), "AddQuestItem", typeof(QuestItem))]
    public class InventoryQuestItem_Patch
    {
        public static void Postfix(QuestItem questItem)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.inventoryItems)
            {
                // Dont send upgraded versions of the chalice
                if (questItem.id == "QI76" || questItem.id == "QI77") return;

                Main.Multiplayer.obtainedGameProgress(questItem.id, ProgressManager.ProgressType.QuestItem, 0);
            }
        }
    }
    [HarmonyPatch(typeof(InventoryManager), "RemoveQuestItem", typeof(QuestItem))]
    public class InventoryQuestItemRemove_Patch
    {
        public static void Postfix(QuestItem questItem)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.inventoryItems)
            {
                // Dont remove versions of the chalice
                if (questItem.id == "QI75" || questItem.id == "QI76" || questItem.id == "QI77") return;

                Main.Multiplayer.obtainedGameProgress(questItem.id, ProgressManager.ProgressType.QuestItem, 1);
            }
        }
    }

    // Player stats

    [HarmonyPatch(typeof(Attribute), "Upgrade")]
    public class AttributeUpgrade_Patch
    {
        public static bool Prefix(Attribute __instance)
        {
            // If receiving upgrade, then just upgrade and skip send
            if (ProgressManager.updatingProgress)
                return true;

            // If shouldn't upgrade because received upgrade from same room, skip upgrade and skip send
            if (!Main.Multiplayer.CanObtainStatUpgrades)
                return false;

            // If you can upgrade and found this naturally, upgrade and send
            string type = null;
            if (__instance.GetType() == typeof(Life)) type = "LIFE";
            else if (__instance.GetType() == typeof(Fervour)) type = "FERVOUR";
            else if (__instance.GetType() == typeof(Strength)) type = "STRENGTH";
            else if (__instance.GetType() == typeof(MeaCulpa)) type = "MEACULPA";
            else if (__instance.GetType() == typeof(BeadSlots)) type = "BEADSLOTS";
            else if (__instance.GetType() == typeof(Flask)) type = "FLASK";
            else if (__instance.GetType() == typeof(FlaskHealth)) type = "FLASKHEALTH";
            byte upgradeLevel = (byte)(__instance.GetUpgrades() + 1);

            if (type != null && upgradeLevel > 1 && Main.Multiplayer.config.syncSettings.playerStats)
                Main.Multiplayer.obtainedGameProgress(type, ProgressManager.ProgressType.PlayerStat, upgradeLevel);
            return true;
        }
    }

    // Sword skills

    [HarmonyPatch(typeof(SkillManager), "UnlockSkill")]
    public class SkilManager_Patch
    {
        public static bool Prefix(string skill, bool ignoreChecks, Dictionary<string, UnlockableSkill> ___allSkills)
        {
            if (ProgressManager.updatingProgress)
            {
                // Just received this skill from another player, skip checks & cost
                ___allSkills[skill].unlocked = true;
                return false;
            }

            if (!Main.Multiplayer.inRando || ignoreChecks)
            {
                // Actually obtaining item, send to other players
                if (Main.Multiplayer.config.syncSettings.swordSkills)
                    Main.Multiplayer.obtainedGameProgress(skill, ProgressManager.ProgressType.SwordSkill, 0);
            }
            return true;
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
                // Not syncing map cells
                if (!Main.Multiplayer.config.syncSettings.mapCells)
                    return true;

                foreach (CellData cell in ___CurrentMap.CellsByZone[__instance.CurrentScene])
                {
                    if (cell.Bounding.Contains(position) && !cell.Revealed)
                        Main.Multiplayer.obtainedGameProgress(___CurrentMap.Cells.IndexOf(cell).ToString(), ProgressManager.ProgressType.MapCell, 0);
                }
                return true;
            }

            // Received this new cell from other player, skip other stuff
            int cellIdx = Mathf.RoundToInt(position.x);
            if (___CurrentMap != null && cellIdx >= 0 && cellIdx < ___CurrentMap.Cells.Count)
            {
                ___CurrentMap.Cells[cellIdx].Revealed = true;
            }
            return false;
        }
    }

    // Game flags

    [HarmonyPatch(typeof(EventManager), "SetFlag")]
    public class EventManager_Patch
    {
        public static void Postfix(EventManager __instance, string id, bool b)
        {
            string formatted = __instance.GetFormattedId(id);
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.worldState && FlagStates.getFlagState(formatted) != null)
            {
                Main.Multiplayer.obtainedGameProgress(formatted, ProgressManager.ProgressType.Flag, (byte)(b ? 0 : 1));
            }
        }
    }

    // Warp teleports

    [HarmonyPatch(typeof(SpawnManager), "SetTeleportActive")]
    public class SpawnManager_Patch
    {
        public static void Postfix(string teleportId)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.worldState)
            {
                Main.Multiplayer.obtainedGameProgress(teleportId, ProgressManager.ProgressType.Teleport, 0);
            }
        }
    }

    // Church donations

    [HarmonyPatch(typeof(AlmsManager), "ConsumeTears")]
    public class AlmsManager_Patch
    {
        public static void Postfix(AlmsManager __instance)
        {
            if (!ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.worldState)
            {
                Main.Multiplayer.obtainedGameProgress("Tears", ProgressManager.ProgressType.ChurchDonation, (byte)(__instance.TearsGiven / 1000));
            }
        }
    }

    // Miriam status

    // Send miriam closed portal
    [HarmonyPatch(typeof(EventManager), "EndMiriamPortalAndReturn")]
    public class EventManagerMiriamPortal_Patch
    {
        public static void Prefix(EventManager __instance)
        {
            if (__instance.AreInMiriamLevel() && !ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.worldState)
            {
                Main.Multiplayer.obtainedGameProgress(__instance.MiriamCurrentScenePortal, ProgressManager.ProgressType.MiriamStatus, 0);
            }
        }
    }

    // Send miriam quest start
    [HarmonyPatch(typeof(EventManager), "StartMiriamQuest")]
    public class EventManagerMiriamStart_Patch
    {
        public static void Postfix(bool __result)
        {
            if (__result && !ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.worldState)
            {
                Main.Multiplayer.obtainedGameProgress("START", ProgressManager.ProgressType.MiriamStatus, 0);
            }
        }
    }

    // Send miriam quest finish
    [HarmonyPatch(typeof(EventManager), "FinishMiriamQuest")]
    public class EventManagerMiriamFinish_Patch
    {
        public static void Postfix(bool __result)
        {
            if (__result && !ProgressManager.updatingProgress && Main.Multiplayer.config.syncSettings.worldState)
            {
                Main.Multiplayer.obtainedGameProgress("FINISH", ProgressManager.ProgressType.MiriamStatus, 0);
            }
        }
    }

    // Receive closed portal
    [HarmonyPatch(typeof(EventManager), "SetCurrentPersistentState")]
    public class EventManagerMiriamReceive_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, string dataPath, List<string> ___MiriamClosedPortals)
        {
            if (data == null)
            {
                // Called specially - close specified portal
                if (!___MiriamClosedPortals.Contains(dataPath))
                    ___MiriamClosedPortals.Add(dataPath);
                return false;
            }
            else
            {
                // Called normally
                return true;
            }
        }
    }

    // Persistent objects

    // Each type has three methods:
    // 1. Use - When actually using an object it will send the data to other players
    // 2. Receive - When receiving object data it will play the used animation
    // 3. Load - When loading a new scene it will be automatically used

    // Interactable use (PrieDieus, CollectibleItems, Chests, Levers, Customs)
    [HarmonyPatch(typeof(Interactable), "Use")] // Change to patches for each type of pers. object
    public class InteractableUse_Patch
    {
        public static void Postfix(Interactable __instance)
        {
            if (ProgressManager.updatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log($"Used {__instance.GetType()}: {persistentId}");
            Main.Multiplayer.progressManager.usePersistentObject(persistentId);
        }
    }

    // Prie Dieu
    // Interactable use
    [HarmonyPatch(typeof(PrieDieu), "GetCurrentPersistentState")]
    public class PrieDieuReceive_Patch
    {
        public static bool Prefix(string dataPath, PrieDieu __instance)
        {
            if (dataPath != "use") return true;

            // Maybe play activation animation
            __instance.Ligthed = true;
            return false;
        }
    }
    [HarmonyPatch(typeof(PrieDieu), "SetCurrentPersistentState")]
    public class PrieDieuLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, PrieDieu __instance)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            __instance.Ligthed = true;
            return false;
        }
    }

    // Collectible item
    // Interactable use
    [HarmonyPatch(typeof(CollectibleItem), "GetCurrentPersistentState")]
    public class CollectibleItemReceive_Patch
    {
        public static bool Prefix(string dataPath, CollectibleItem __instance, Animator ___interactableAnimator)
        {
            if (dataPath != "use") return true;

            __instance.Consumed = true;
            ___interactableAnimator.gameObject.SetActive(false);
            return false;
        }
    }
    [HarmonyPatch(typeof(CollectibleItem), "SetCurrentPersistentState")]
    public class CollectibleItemLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, CollectibleItem __instance, Animator ___interactableAnimator)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            __instance.Consumed = true;
            ___interactableAnimator.gameObject.SetActive(false);
            return false;
        }
    }

    // Chest
    // Interactable use
    [HarmonyPatch(typeof(Chest), "GetCurrentPersistentState")]
    public class ChestReceive_Patch
    {
        public static bool Prefix(string dataPath, Chest __instance, Animator ___interactableAnimator)
        {
            if (dataPath != "use") return true;

            __instance.Consumed = true;
            ___interactableAnimator.SetBool("USED", true);
            return false;
        }
    }
    [HarmonyPatch(typeof(Chest), "SetCurrentPersistentState")]
    public class ChestLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, Chest __instance, Animator ___interactableAnimator)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            __instance.Consumed = true;
            ___interactableAnimator.SetBool("NOANIMUSED", true);
            return false;
        }
    }

    // Cherub
    [HarmonyPatch(typeof(CherubCaptorPersistentObject), "OnCherubKilled")]
    public class CherubUse_Patch
    {
        public static void Postfix(CherubCaptorPersistentObject __instance)
        {
            if (ProgressManager.updatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Cherub killed: " + persistentId);
            Main.Multiplayer.progressManager.usePersistentObject(persistentId);
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
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            __instance.destroyed = true;
            __instance.spawner.DisableCherubSpawn();
            __instance.spawner.DestroySpawnedCherub();
            return false;
        }
    }

    // Lever
    // Interactable use
    [HarmonyPatch(typeof(Lever), "GetCurrentPersistentState")]
    public class LeverReceive_Patch
    {
        public static bool Prefix(string dataPath, Lever __instance, Animator ___interactableAnimator)
        {
            if (dataPath != "use") return true;

            __instance.Consumed = true;
            ___interactableAnimator.SetBool("ACTIVE", true); // Might still activate other objects
            return false;
        }
    }
    [HarmonyPatch(typeof(Lever), "SetCurrentPersistentState")]
    public class LeverLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, Lever __instance)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            __instance.Consumed = true;
            __instance.SetLeverDownInstantly();
            return false;
        }
    }

    // Gate
    [HarmonyPatch(typeof(Gate), "Use")]
    public class GateUse_Patch
    {
        public static void Postfix(Gate __instance)
        {
            if (ProgressManager.updatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Gate opened: " + persistentId);
            Main.Multiplayer.progressManager.usePersistentObject(persistentId);
        }
    }
    [HarmonyPatch(typeof(Gate), "GetCurrentPersistentState")]
    public class GateReceive_Patch
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
    public class GateLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, Gate __instance, ref bool ___open, Animator ___animator, Collider2D ___collision)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            ___open = true;
            ___collision.enabled = false;
            ___animator.SetBool("INSTA_ACTION", true);
            ___animator.SetBool("OPEN", true);
            return false;
        }
    }

    // Moving platform
    [HarmonyPatch(typeof(StraightMovingPlatform), "Use")]
    public class MovingPlatformUse_Patch
    {
        public static void Postfix(StraightMovingPlatform __instance, ref bool ____running)
        {
            if (ProgressManager.updatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Activated platform: " + persistentId);
            Main.Multiplayer.progressManager.usePersistentObject(persistentId);

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
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            __instance.Reset();
            __instance.Use();
            return false;
        }
    }

    // Slash trigger
    [HarmonyPatch(typeof(TriggerReceiver), "Use")]
    public class SlashTriggerUse_Patch
    {
        public static void Postfix(TriggerReceiver __instance)
        {
            if (ProgressManager.updatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Trigger activated: " + persistentId);
            Main.Multiplayer.progressManager.usePersistentObject(persistentId);
        }
    }
    [HarmonyPatch(typeof(TriggerReceiver), "GetCurrentPersistentState")]
    public class SlashTriggerReceive_Patch
    {
        public static bool Prefix(string dataPath, TriggerReceiver __instance, ref bool ___alreadyUsed)
        {
            if (dataPath != "use") return true;

            ___alreadyUsed = true;
            __instance.animator.SetTrigger("ACTIVATE");
            Collider2D collider = __instance.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            return false;
        }
    }
    [HarmonyPatch(typeof(TriggerReceiver), "SetCurrentPersistentState")]
    public class SlashTriggerLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, TriggerReceiver __instance, ref bool ___alreadyUsed)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            ___alreadyUsed = true;
            __instance.animator.Play("USED");
            Collider2D collider = __instance.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;
            return false;
        }
    }

    // Breakable wall
    [HarmonyPatch(typeof(BreakableWall), "Damage")]
    public class BreakableWallUse_Patch
    {
        public static void Postfix(BreakableWall __instance)
        {
            if (ProgressManager.updatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Broke wall: " + persistentId);
            Main.Multiplayer.progressManager.usePersistentObject(persistentId);
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
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            __instance.Use(); // Needs to be changed
            return false;
        }
    }

    // Moving ladder
    [HarmonyPatch(typeof(ActionableLadder), "Use")]
    public class LadderUse_Patch
    {
        public static void Postfix(ActionableLadder __instance)
        {
            if (ProgressManager.updatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Ladder activated: " + persistentId);
            Main.Multiplayer.progressManager.usePersistentObject(persistentId);
        }
    }
    [HarmonyPatch(typeof(ActionableLadder), "GetCurrentPersistentState")]
    public class LadderReceive_Patch
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
    public class LadderLoad_Patch
    {
        public static bool Prefix(PersistentManager.PersistentData data, ActionableLadder __instance, ref bool ___open)
        {
            if (data != null)
            {
                // This method is being called normally - only execute if object hasn't been interacted with
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            ___open = false;
            __instance.Use(); // Needs to be changed
            return false;
        }
    }

    // Door
    [HarmonyPatch(typeof(Door), "EnterDoor")]
    public class DoorUse_Patch
    {
        public static void Postfix(Door __instance)
        {
            if (ProgressManager.updatingProgress) return;

            string persistentId = __instance.GetPersistenID();
            Main.Multiplayer.Log("Door opened: " + persistentId);
            Main.Multiplayer.progressManager.usePersistentObject(persistentId);
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
                return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
            }

            ___objectUsed = true;
            __instance.Closed = false;
            ___interactableAnimator.SetTrigger("INSTA_OPEN");
            return false;
        }
    }

    // Custom interaction
    // Interactable use
    //[HarmonyPatch(typeof(CustomInteraction), "GetCurrentPersistentState")]
    //public class CustomReceive_Patch
    //{
    //    public static bool Prefix(string dataPath, CustomInteraction __instance, Animator ___interactableAnimator)
    //    {
    //        if (dataPath != "use") return true;

    //        __instance.Consumed = true;
    //        ___interactableAnimator.SetTrigger("INTERACTED");
    //        return false;
    //    }
    //}
    //[HarmonyPatch(typeof(CustomInteraction), "SetCurrentPersistentState")]
    //public class CustomLoad_Patch
    //{
    //    public static bool Prefix(PersistentManager.PersistentData data, CustomInteraction __instance)
    //    {
    //        if (data != null)
    //        {
    //            // This method is being called normally - only execute if object hasn't been interacted with
    //            return !Main.Multiplayer.checkPersistentObject(__instance.GetPersistenID());
    //        }

    //        __instance.Consumed = true;
    //        // Play instant animation
    //        return false;
    //    }
    //}

    // Hidden secrets

    // Initial data sending

    // Skills
    [HarmonyPatch(typeof(SkillManager), "GetCurrentPersistentState")]
    public class SkillManagerIntro_Patch
    {
        public static bool Prefix(string dataPath, Dictionary<string, UnlockableSkill> ___allSkills)
        {
            // Calling this with 'intro' means it should send all unlocked skills
            if (dataPath != "intro") return true;

            foreach (string key in ___allSkills.Keys)
            {
                if (___allSkills[key].unlocked)
                    Main.Multiplayer.obtainedGameProgress(key, ProgressManager.ProgressType.SwordSkill, 0);
            }
            return false;
        }
    }

    // Map cells
    [HarmonyPatch(typeof(NewMapManager), "GetCurrentPersistentState")]
    public class MapManagerIntro_Patch
    {
        public static bool Prefix(string dataPath, MapData ___CurrentMap)
        {
            // Calling this with 'intro' means it should send all revealed map cells
            if (dataPath != "intro") return true;

            for (int i = 0; i < ___CurrentMap.Cells.Count; i++)
            {
                if (___CurrentMap.Cells[i].Revealed)
                    Main.Multiplayer.obtainedGameProgress(i.ToString(), ProgressManager.ProgressType.MapCell, 0);
            }
            return false;
        }
    }

    // Teleports
    [HarmonyPatch(typeof(SpawnManager), "GetCurrentPersistentState")]
    public class SpawnManagerIntro_Patch
    {
        public static bool Prefix(string dataPath, Dictionary<string, TeleportDestination> ___TeleportDict)
        {
            // Calling this with 'intro' means it should send all unlocked teleports
            if (dataPath != "intro") return true;

            foreach (string key in ___TeleportDict.Keys)
            {
                if (___TeleportDict[key].isActive)
                    Main.Multiplayer.obtainedGameProgress(key, ProgressManager.ProgressType.Teleport, 0);
            }
            return false;
        }
    }

    // Flags & Miriam status
    [HarmonyPatch(typeof(EventManager), "GetCurrentPersistentState")]
    public class EventManagerIntro_Patch
    {
        public static bool Prefix(string dataPath, Dictionary<string, FlagObject> ___flags, List<string> ___MiriamClosedPortals)
        {
            // Calling this with 'intro' means it should send all set flags
            if (dataPath != "intro") return true;

            foreach (string flag in ___flags.Keys)
            {
                if (FlagStates.getFlagState(flag) != null && ___flags[flag].value)
                    Main.Multiplayer.obtainedGameProgress(flag, ProgressManager.ProgressType.Flag, 0);
            }
            foreach (string portal in ___MiriamClosedPortals)
            {
                Main.Multiplayer.obtainedGameProgress(portal, ProgressManager.ProgressType.MiriamStatus, 0);
            }
            return false;
        }
    }


    // Temporarily allow teleportation
    //[HarmonyPatch(typeof(AlmsManager), "GetPrieDieuLevel")]
    //public class AlmsManager_Patch
    //{
    //    public static bool Prefix(ref int __result)
    //    {
    //        __result = 3;
    //        return false;
    //    }
    //}
}
