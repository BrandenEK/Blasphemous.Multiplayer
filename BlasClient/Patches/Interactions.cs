using HarmonyLib;
using Tools.Level.Interactables;

namespace BlasClient.Patches
{
    // Collectible item animation
    [HarmonyPatch(typeof(CollectibleItem), "OnUse")]
    public class CollecibleItemUse_Patch
    {
        public static void Prefix(CollectibleItem.CollectibleHeight ___height)
        {
            Main.UnityLog("On use corroutine");
            Main.Multiplayer.usingSpecialAnimation((byte)(___height == CollectibleItem.CollectibleHeight.Floor ? 240 : 241));
        }
    }

    // Chest animation
    [HarmonyPatch(typeof(Chest), "OnUse")]
    public class ChestUse_Patch
    {
        public static void Prefix()
        {
            Main.UnityLog("On use corroutine");
            Main.Multiplayer.usingSpecialAnimation(242);
        }
    }

    // Prie Dieu animation
    [HarmonyPatch(typeof(PrieDieu), "OnUse")]
    public class PrieDieuUse_Patch
    {
        public static void Prefix(PrieDieu __instance)
        {
            Main.UnityLog("On use corroutine");
            Main.Multiplayer.usingSpecialAnimation((byte)(__instance.Ligthed ? 244 : 243));
        }
    }
}
