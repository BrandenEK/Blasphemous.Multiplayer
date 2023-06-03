using Gameplay.GameControllers.Entities;

namespace BlasClient.PvP
{
    [System.Serializable]
    public class PlayerAttack
    {
        public string AttackName { get; set; }
        public byte DamageType { get; set; }
        public byte DamageElement { get; set; }

        public int BaseDamage { get; set; }
        public int Force { get; set; }
        public string SoundId { get; set; }

        public DamageArea.DamageType GetDamageType() => (DamageArea.DamageType)DamageType;

        public DamageArea.DamageElement GetDamageElement() => (DamageArea.DamageElement)DamageElement;

        public AttackType GetAttackType() => (AttackType)System.Enum.Parse(typeof(AttackType), AttackName);
    }
}
