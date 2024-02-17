using Framework.Managers;
using System.Collections;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Players
{
    public class PlayerStatus
    {
        // Name of this player.  Used as unique id
        public string Name { get; private set; }

        // Current scene to determine if in same scene
        private string _currentScene;
        public string CurrentScene
        {
            get => _currentScene;
            set
            {
                _currentScene = value;
                if (value.Length == 9)
                    _lastMapScene = value;
            }
        }

        // Last valid scene to show on map
        private string _lastMapScene;
        public string LastMapScene => _lastMapScene;

        // Current team of this player.  Used to determine map icons & pvp availability
        public byte Team { get; set; }

        // Animation id when this player is using a special animation
        public byte SpecialAnimation { get; set; }

        // Skin sprite, is updated whenever receiving a skin update packet
        private Texture2D _skinTexture;
        public Texture2D SkinTexture => _skinTexture;

        // Determines whether to set player objects skin texture in an update cycle
        public SkinStatus SkinUpdateStatus { get; set; }

        public PlayerStatus(string name)
        {
            Name = name;
            Team = 1;
            SpecialAnimation = 0;
            _currentScene = string.Empty;
            _lastMapScene = string.Empty;

            SetSkinTexture("PENITENT_DEFAULT");
            SkinUpdateStatus = SkinStatus.Updated;
        }

        public void SetSkinTexture(string skinName)
        {
            _skinTexture = Core.ColorPaletteManager.GetColorPaletteById(skinName).texture;
        }

        public void SetSkinTexture(byte[] skinData)
        {
            Main.Instance.StartCoroutine(DelaySkinUpdate());

            IEnumerator DelaySkinUpdate()
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                Texture2D tex = new Texture2D(256, 1, TextureFormat.RGB24, false);
                tex.LoadImage(skinData);
                tex.filterMode = FilterMode.Point;
                _skinTexture = tex;
            }
        }

        public static bool IsOriginalSkin(string skinName)
        {
            foreach (string ogskin in originalSkins)
            {
                if (skinName == ogskin)
                    return true;
            }
            return false;
        }

        private static readonly string[] originalSkins = new string[]
        {
            "PENITENT_DEFAULT",
            "PENITENT_ENDING_A",
            "PENITENT_ENDING_B",
            "PENITENT_OSSUARY",
            "PENITENT_BACKER",
            "PENITENT_DELUXE",
            "PENITENT_ALMS",
            "PENITENT_PE01",
            "PENITENT_PE02",
            "PENITENT_PE03",
            "PENITENT_BOSSRUSH",
            "PENITENT_DEMAKE",
            "PENITENT_ENDING_C",
            "PENITENT_SIERPES",
            "PENITENT_ISIDORA",
            "PENITENT_BOSSRUSH_S",
            "PENITENT_GAMEBOY",
            "PENITENT_KONAMI"
        };

        public enum SkinStatus
        {
            Updated = 0,
            YesUpdate = 1,
            NoUpdate = 2,
        }
    }
}
