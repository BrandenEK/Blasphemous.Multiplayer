using Gameplay.GameControllers.Entities;

namespace BlasClient.PvP
{
    [System.Serializable]
    public class PlayerAttack
    {
        public string AttackName { get; set; }
        public string DamageType { get; set; }
        public string DamageElement { get; set; }

        public int BaseDamage { get; set; }
        public int DamageScaling { get; set; }
        public int Force { get; set; }
        public string SoundId { get; set; }

        public DamageArea.DamageType GetDamageType() => (DamageArea.DamageType)System.Enum.Parse(typeof(DamageArea.DamageType), DamageType);

        public DamageArea.DamageElement GetDamageElement() => (DamageArea.DamageElement)System.Enum.Parse(typeof(DamageArea.DamageElement), DamageElement);

        public AttackType GetAttackType() => (AttackType)System.Enum.Parse(typeof(AttackType), AttackName);
    }
}
