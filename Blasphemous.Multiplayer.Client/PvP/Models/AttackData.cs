using Gameplay.GameControllers.Entities;

namespace Blasphemous.Multiplayer.Client.PvP.Models
{
    public class AttackData(AttackType name, DamageArea.DamageType damageType, DamageArea.DamageElement damageElement, int baseDamage, ScalingType scalingType, int scalingAmount, int force, string soundId)
    {
        public AttackType Name { get; } = name;
        public DamageArea.DamageType DamageType { get; } = damageType;
        public DamageArea.DamageElement DamageElement { get; } = damageElement;

        public int BaseDamage { get; } = baseDamage;
        public ScalingType ScalingType { get; } = scalingType;
        public int ScalingAmount { get; } = scalingAmount;

        public int Force { get; } = force;
        public string SoundId { get; } = soundId;
    }
}
