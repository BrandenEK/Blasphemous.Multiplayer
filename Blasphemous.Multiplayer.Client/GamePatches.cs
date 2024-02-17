using HarmonyLib;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Penitent.Damage;

namespace Blasphemous.Multiplayer.Client
{    
    // Send updated skin when picking a new one
    [HarmonyPatch(typeof(ColorPaletteManager), "SetCurrentColorPaletteId")]
    public class ColorPaletteManager_Patch
    {
        public static void Postfix(string colorPaletteId)
        {
            Main.Multiplayer.NetworkManager.SendSkin(colorPaletteId);
        }
    }

    // Prevent crash if thrown back out of scene
    [HarmonyPatch(typeof(ThrowBack), "OnDestroy")]
    public class ThrowBack_Patch
    {
        public static bool Prefix(PenitentDamageArea ____damageArea)
        {
            if (____damageArea != null && ____damageArea.OnDamaged == null)
            {
                Main.Multiplayer.LogWarning("OnDamaged was null!");
                return false;
            }
            return true;
        }
    }
}
