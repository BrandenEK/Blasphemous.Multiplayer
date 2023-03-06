using HarmonyLib;
using Framework.Managers;
using UnityEngine;

namespace BlasClient.Patches
{    
    // Send updated skin when picking a new one
    [HarmonyPatch(typeof(ColorPaletteManager), "SetCurrentColorPaletteId")]
    public class ColorPaletteManager_Patch
    {
        public static void Postfix(string colorPaletteId)
        {
            Sprite newSkin = Core.ColorPaletteManager.GetColorPaletteById(colorPaletteId);
            Main.Multiplayer.changeSkin(newSkin.texture.GetRawTextureData());
        }
    }
}
