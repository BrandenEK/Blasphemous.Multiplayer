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
            Texture2D tex = new Texture2D(256, 1, TextureFormat.RGB24, false);
            tex.LoadRawTextureData(skin);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            skinTexture = tex;
        }
    }
}
