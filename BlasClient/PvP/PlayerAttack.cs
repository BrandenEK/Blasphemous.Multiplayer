using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace BlasClient.PvP
{
    public abstract class PlayerAttack
    {
        public abstract DamageArea.DamageType DamageType { get; }
        public abstract int BaseDamage { get; }
        public abstract int Force { get; }
    }

    public class NormalAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Normal;

        public override int BaseDamage => 10;

        public override int Force => 1;
    }

    public class ChargedAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Heavy;

        public override int BaseDamage => 40;

        public override int Force => 2;
    }

    public class LungeAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Heavy;

        public override int BaseDamage => 30;

        public override int Force => 1;
    }

    public class VerticalAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Heavy;

        public override int BaseDamage => 20;

        public override int Force => 1;
    }

    public class RangedAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Normal;

        public override int BaseDamage => 20;

        public override int Force => 1;
    }

    public class DeblaAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Normal;

        public override int BaseDamage => 60;

        public override int Force => 1;
    }
}
