using Blasphemous.ModdingAPI;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Damage;
using HarmonyLib;

namespace Blasphemous.Multiplayer.Client
{    
    // Send updated skin when picking a new one
    [HarmonyPatch(typeof(ColorPaletteManager), nameof(ColorPaletteManager.SetCurrentColorPaletteId))]
    class ColorPaletteManager_Patch
    {
        public static void Postfix(string colorPaletteId)
        {
            Main.Multiplayer.NetworkManager.SendSkin(colorPaletteId);
        }
    }

    // Prevent crash if thrown back out of scene
    [HarmonyPatch(typeof(ThrowBack), nameof(ThrowBack.OnDestroy))]
    class ThrowBack_Patch
    {
        public static bool Prefix(PenitentDamageArea ____damageArea)
        {
            if (____damageArea != null && ____damageArea.OnDamaged == null)
            {
                ModLog.Warn("OnDamaged was null!");
                return false;
            }
            return true;
        }
    }
}
