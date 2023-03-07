using UnityEngine;
using Framework.Managers;

namespace BlasClient.Structures
{
    public class SkinStatus
    {
        // Skin sprite, is updated whenever receiving a skin update packet
        public Texture2D skinTexture;
        // Determines whether to set player objects skin texture in an update cycle
        // 0 - Already updated, do nothing
        // 1 - When object is first created
        // 2 - First update cycle
        public byte updateStatus;

        public SkinStatus()
        {
            skinTexture = Core.ColorPaletteManager.GetColorPaletteById("PENITENT_DEFAULT").texture;
        }

        public void createSkin(byte[] skin)
        {
            // A small skinData means an original skin (Or error)
            if (skin.Length < 64)
            {
                string skinName = System.Text.Encoding.UTF8.GetString(skin);
                skinTexture = Core.ColorPaletteManager.GetColorPaletteById(skinName).texture;
                return;
            }

            Texture2D tex = new Texture2D(256, 1, TextureFormat.RGB24, false);
            tex.LoadImage(skin);
            tex.filterMode = FilterMode.Point;
            skinTexture = tex;
        }
    }
}
