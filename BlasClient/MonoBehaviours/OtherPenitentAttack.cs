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

        public void CreatePenitentAttack(RuntimeAnimatorController swordController)
        {
            // Add components
            transform.localPosition = Vector3.zero;
            SwordRenderer = gameObject.AddComponent<SpriteRenderer>();
            SwordAnim = gameObject.AddComponent<Animator>();
            SwordAnim.runtimeAnimatorController = swordController;

            // Create damage area collider
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
            collider.offset = new Vector2(0, 0.92f);
            collider.size = new Vector2(0.665f, 1.866f);
            collider.isTrigger = true;
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
    }
}
