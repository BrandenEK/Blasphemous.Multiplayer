using Gameplay.GameControllers.Entities;

namespace Blasphemous.Multiplayer.Client.PvP.Models
{
    public class AttackData(AttackType name, DamageArea.DamageType damageType, DamageArea.DamageElement damageElement, float baseDamage, ScalingType scalingType, float scalingAmount, float force, string soundId)
    {
        public AttackType Name { get; } = name;
        public DamageArea.DamageType DamageType { get; } = damageType;
        public DamageArea.DamageElement DamageElement { get; } = damageElement;

        public float BaseDamage { get; } = baseDamage;
        public ScalingType ScalingType { get; } = scalingType;
        public float ScalingAmount { get; } = scalingAmount;

        public float Force { get; } = force;
        public string SoundId { get; } = soundId;
    }
}
