using HarmonyLib;
using Framework.Inventory;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Abilities;
using Gameplay.GameControllers.Penitent.Damage;

namespace BlasClient.PvP
{
    // Send attack effects when starting to use a prayer
    [HarmonyPatch(typeof(PrayerUse), "StartUsingPrayer")]
    public class PrayerUse_Patch
    {
        public static void Postfix(Prayer prayer)
        {
            switch (prayer.id)
            {
                case "PR03":
                    Main.Multiplayer.SendNewEffect(EffectType.Debla);
                    break;
                //case "PR14":
                //    Main.Multiplayer.SendNewEffect(EffectType.Verdiales);
                //    break;
            }
        }
    }

    // Bypass iframes when getting attacked by another player
    //[HarmonyPatch(typeof(PenitentDamageArea), "TakeDamage")]
    //public class PenitentDamageIframes_Patch
    //{
    //    public static void Prefix(Hit hit, ref float ___DeltaRecoverTime, float ___RecoverTime)
    //    {
    //        if (hit.AttackingEntity != null && hit.AttackingEntity.name.StartsWith("_"))
    //        {
    //            Main.Multiplayer.LogWarning("Bypassing iframes for player attack!");
    //            ___DeltaRecoverTime = ___RecoverTime;
    //        }
    //    }
    //}

    // Prevent knockback when getting attacked by another player
    //[HarmonyPatch(typeof(PenitentDamageArea), "HitDisplacementForce")]
    //public class PenitentDamageKnockback_Patch
    //{
    //    public static bool Prefix(PenitentDamageArea __instance)
    //    {
    //        Main.Multiplayer.LogWarning("Checking for hit displacement skip");
    //        return __instance.LastHit.DamageType == DamageArea.DamageType.Heavy || !__instance.LastHit.AttackingEntity.name.StartsWith("_");
    //    }
    //}
}
