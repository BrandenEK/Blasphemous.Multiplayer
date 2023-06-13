using Framework.FrameworkCore;
using Framework.Managers;
using HarmonyLib;
using System.Collections.Generic;

namespace BlasClient.ProgressSync.Helpers
{
    public class SwordSkillHelper : IProgressHelper
    {
        public void ApplyProgress(ProgressUpdate progress)
        {
            Core.SkillManager.UnlockSkill(progress.Id, true);
        }

        public string GetProgressNotification(ProgressUpdate progress)
        {
            UnlockableSkill skill = Core.SkillManager.GetSkill(progress.Id);
            return skill != null ? $"{Main.Multiplayer.Localize("sklnot")} {skill.caption}" : null;
        }

        public void SendAllProgress()
        {
            Core.SkillManager.GetCurrentPersistentState("intro", false);
        }
    }

    [HarmonyPatch(typeof(SkillManager), "UnlockSkill")]
    public class SkilManager_Patch
    {
        public static bool Prefix(string skill, bool ignoreChecks, Dictionary<string, UnlockableSkill> ___allSkills)
        {
            if (Main.Multiplayer.ProgressManager.CurrentlyUpdatingProgress)
            {
                // Just received this skill from another player, skip checks & cost
                ___allSkills[skill].unlocked = true;
                return false;
            }

            if (!Main.Multiplayer.RandomizerMode || ignoreChecks)
            {
                // Actually obtaining item, send to other players
                ProgressUpdate progress = new ProgressUpdate(skill, ProgressType.SwordSkill, 0);
                Main.Multiplayer.NetworkManager.SendProgress(progress);
            }
            return true;
        }
    }

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
                {
                    ProgressUpdate progress = new ProgressUpdate(key, ProgressType.SwordSkill, 0);
                    Main.Multiplayer.NetworkManager.SendProgress(progress);
                }
            }
            return false;
        }
    }
}
