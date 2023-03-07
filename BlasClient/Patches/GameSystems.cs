using HarmonyLib;
using Framework.Managers;

namespace BlasClient.Patches
{    
    // Send updated skin when picking a new one
    [HarmonyPatch(typeof(ColorPaletteManager), "SetCurrentColorPaletteId")]
    public class ColorPaletteManager_Patch
    {
        public static void Postfix(string colorPaletteId)
        {
            Main.Multiplayer.changeSkin(colorPaletteId);
        }
    }
}
