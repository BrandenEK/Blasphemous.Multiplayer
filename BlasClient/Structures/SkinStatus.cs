using UnityEngine;
using Framework.Managers;

namespace BlasClient.Structures
{
    public class SkinStatus
    {
        // Skin sprite, is updated whenever receiving a skin update packet
        public Sprite skinSprite;
        // Determines whether to set player objects skin texture in an update cycle
        // 0 - Already updated, do nothing
        // 1 - When object is first created
        // 2 - First update cycle
        public byte updateStatus;

        public SkinStatus()
        {
            skinSprite = Core.ColorPaletteManager.GetColorPaletteById("PENITENT_DEFAULT");
        }

        public void createSkin(byte[] skin)
        {
            Texture2D tex = new Texture2D(256, 1, TextureFormat.RGB24, false);
            tex.LoadImage(skin);
            tex.filterMode = FilterMode.Point;
            skinSprite = Sprite.Create(tex, new Rect(0, 0, 256, 1), new Vector2(0.5f, 0.5f));
        }
    }
}
