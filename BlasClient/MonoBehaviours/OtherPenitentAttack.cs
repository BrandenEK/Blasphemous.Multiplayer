using System.Collections;
using UnityEngine;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using BlasClient.PvP;

namespace BlasClient.MonoBehaviours
{
    public class OtherPenitentAttack : MonoBehaviour
    {
        private SpriteRenderer SwordRenderer { get; set; }
        private Animator SwordAnim { get; set; }

        public BoxCollider2D AttackCollider { get; private set; }

        // These variables only last as long as the current attack
        private bool facingRight;
        private bool hitPlayer;

        public void CreatePenitentAttack(RuntimeAnimatorController swordController)
        {
            // Add components
            transform.localPosition = Vector3.zero;
            SwordRenderer = gameObject.AddComponent<SpriteRenderer>();
            SwordAnim = gameObject.AddComponent<Animator>();
            SwordAnim.runtimeAnimatorController = swordController;

            // Create attack area collider
            AttackCollider = gameObject.AddComponent<BoxCollider2D>();
            AttackCollider.offset = new Vector2(0, 0.92f);
            AttackCollider.size = new Vector2(0.665f, 1.866f);
            AttackCollider.isTrigger = true;
        }

        // When receiving an attack from another player, make their character play the sword/prayer animation
        public void PlayAttackAnimation(byte attack, bool facingRight)
        {
            SwordRenderer.flipX = !facingRight;

            switch (attack)
            {
                case 0: SwordAnim.Play("Basic1_Lv1"); break;
                case 1: SwordAnim.Play("BasicUpward_Lv1"); break;
                case 2: SwordAnim.Play("Air1_Lv1"); break;
                case 3: SwordAnim.Play("AirUpward_Lv1"); break;
                case 4: SwordAnim.Play("Crouch_Lv1"); break;
            }
        }

        private bool IsOverlappingPlayer()
        {
            Collider2D[] colliders = Physics2D.OverlapAreaAll(AttackCollider.bounds.min, AttackCollider.bounds.max, 1 << 18);
            foreach (Collider2D col in colliders)
            {
                if (col.gameObject.name == "Body")
                    return true;
            }
            return false;
        }

        public void StartAttack(PlayerAttack attack, bool facingRight)
        {
            this.facingRight = facingRight;

            if (attack.Delay > 0)
            {
                StartCoroutine(DealDamageAfterDelay(attack, attack.Delay));
            }
            else
            {
                DealDamage(attack);
            }

            IEnumerator DealDamageAfterDelay(PlayerAttack attack, float delay)
            {
                yield return new WaitForSecondsRealtime(delay);
                DealDamage(attack);
            }
        }

        private void DealDamage(PlayerAttack attack)
        {
            // Calculate attack area and check if the player is inside of it
            attack.SetDamageArea(AttackCollider, facingRight);
            if (!IsOverlappingPlayer())
                return;

            // Calculate hit data based on attack & parameters
            Hit hit = new Hit()
            {
                AttackingEntity = transform.parent.gameObject,
                DamageElement = DamageArea.DamageElement.Normal,
                ThrowbackDirByOwnerPosition = true,
                Unparriable = true,
                Unblockable = true,
                Unnavoidable = true,
                // Sound
            };
            hit.DamageAmount = attack.BaseDamage;
            hit.DamageType = attack.DamageType;
            hit.Force = attack.Force;

            // Actually damage player
            Core.Logic.Penitent.Damage(hit);
            Main.Multiplayer.SendNewAttack(255);
        }
    }
}
