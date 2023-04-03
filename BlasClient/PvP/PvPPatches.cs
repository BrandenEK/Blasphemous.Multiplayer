using HarmonyLib;
using Framework.Inventory;
using Gameplay.GameControllers.Penitent.Abilities;

namespace BlasClient.PvP
{
    [HarmonyPatch(typeof(PrayerUse), "StartUsingPrayer")]
    public class PrayerUse_Patch
    {
        public static void Postfix(Prayer prayer)
        {
            switch (prayer.id)
            {
                case "PR03":
                    Main.Multiplayer.LogError("Sending debla effect!");
                    Main.Multiplayer.SendNewEffect(EffectType.Debla);
                    break;
            }
        }
    }
}
