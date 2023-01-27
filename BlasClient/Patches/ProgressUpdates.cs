using HarmonyLib;
using Framework.Managers;
using Framework.Inventory;
using Framework.FrameworkCore;
using Framework.FrameworkCore.Attributes;
using Framework.FrameworkCore.Attributes.Logic;
using System.Collections.Generic;

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
}
