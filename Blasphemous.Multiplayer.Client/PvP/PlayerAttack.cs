using Gameplay.GameControllers.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blasphemous.Multiplayer.Client.PvP
{
    [System.Serializable]
    public class PlayerAttack
    {
        [JsonConverter(typeof(StringEnumConverter))] public AttackType AttackName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] public DamageArea.DamageType DamageType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] public DamageArea.DamageElement DamageElement { get; set; }

        public int BaseDamage { get; set; }
        public int DamageScaling { get; set; }
        public int Force { get; set; }
        public string SoundId { get; set; }
    }
}
