using UnityEngine;

namespace BlasClient.MonoBehaviours
{
    public class OtherPenitentAttack : MonoBehaviour
    {
        private SpriteRenderer SwordRenderer { get; set; }
        private Animator SwordAnim { get; set; }
        private BoxCollider2D DamageArea { get; set; }

        public void CreatePenitentAttack(RuntimeAnimatorController swordController)
        {
            // Add components
            transform.localPosition = Vector3.zero;
            SwordRenderer = gameObject.AddComponent<SpriteRenderer>();
            SwordAnim = gameObject.AddComponent<Animator>();
            SwordAnim.runtimeAnimatorController = swordController;

            // Create damage area collider
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            DamageArea = gameObject.AddComponent<BoxCollider2D>();
            DamageArea.offset = new Vector2(0, 0.92f);
            DamageArea.size = new Vector2(0.665f, 1.866f);
            DamageArea.isTrigger = true;
        }

        // Upon death, the hitbox should be disabled
        public void SetHitboxStatus(bool active)
        {
            DamageArea.enabled = active;
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
                case 20:
                    Main.Multiplayer.LogError("Spawning range attack effect!");
                    break;
            }
        }        
    }
}
