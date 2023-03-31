using Gameplay.GameControllers.Entities;
using UnityEngine;

namespace BlasClient.PvP
{
    public abstract class PlayerAttack
    {
        public abstract void SetDamageArea(BoxCollider2D collider, bool facingRight);

        public abstract DamageArea.DamageType DamageType { get; }
        public abstract int BaseDamage { get; }
        public abstract int Force { get; }
        public abstract float Delay { get; }
    }

    public class SidewaysAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Normal;

        public override int BaseDamage => 30;

        public override int Force => 1;

        public override float Delay => 0.15f;

        public override void SetDamageArea(BoxCollider2D collider, bool facingRight)
        {
            collider.size = new Vector2(2.8f, 1);
            collider.offset = new Vector2(facingRight ? 1.3f : -1.3f, 1.175f);
        }
    }

    public class UpwardsAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Normal;

        public override int BaseDamage => 30;

        public override int Force => 1;

        public override float Delay => 0.15f;

        public override void SetDamageArea(BoxCollider2D collider, bool facingRight)
        {
            collider.size = new Vector2(1.4f, 2.8f);
            collider.offset = new Vector2(0, 2);
        }
    }

    public class CrouchAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Normal;

        public override int BaseDamage => 30;

        public override int Force => 1;

        public override float Delay => 0.15f;

        public override void SetDamageArea(BoxCollider2D collider, bool facingRight)
        {
            collider.size = new Vector2(2.8f, 1);
            collider.offset = new Vector2(facingRight ? 1.3f : -1.3f, 0.5f);
        }
    }

    public class ChargedAttack : PlayerAttack
    {
        public override DamageArea.DamageType DamageType => DamageArea.DamageType.Heavy;

        public override int BaseDamage => 30;

        public override int Force => 2;

        public override float Delay => 0.35f;

        public override void SetDamageArea(BoxCollider2D collider, bool facingRight)
        {
            collider.size = new Vector2(2.4f, 3f);
            collider.offset = new Vector2(facingRight ? 1.6f : -1.6f, 1.5f);
        }
    }
}
