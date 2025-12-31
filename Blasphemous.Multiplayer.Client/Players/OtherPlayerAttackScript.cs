using Blasphemous.ModdingAPI;
using Blasphemous.Multiplayer.Client.PvP.Models;
using Framework.Managers;
using Gameplay.GameControllers.Enemies.Projectiles;
using Gameplay.GameControllers.Entities;
using System.Collections;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Players
{
    public class OtherPlayerAttackScript : MonoBehaviour
    {
        private const float DEBLA_DURATION = 1.2f;
        private const float VERDIALES_DURATION = 2f;

        private SpriteRenderer SwordRenderer { get; set; }
        private Animator SwordAnim { get; set; }
        private BoxCollider2D DamageArea { get; set; }

        public void SetupAttack()
        {
            // Add components
            transform.localPosition = Vector3.zero;
            SwordRenderer = gameObject.AddComponent<SpriteRenderer>();
            SwordAnim = gameObject.AddComponent<Animator>();
            SwordAnim.runtimeAnimatorController = UnityReferences.PlayerSwordAnimator;

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
        public void PlayEffectAnimation(EffectType effect, bool facingRight)
        {
            SwordRenderer.flipX = !facingRight;

            switch (effect)
            {
                case EffectType.SidewaysGrounded:
                    SwordAnim.Play("Basic1_Lv1");
                    break;
                case EffectType.UpwardsGrounded:
                    SwordAnim.Play("BasicUpward_Lv1");
                    break;
                case EffectType.SidewaysAir:
                    SwordAnim.Play("Air1_Lv1");
                    break;
                case EffectType.UpwardsAir:
                    SwordAnim.Play("AirUpward_Lv1");
                    break;
                case EffectType.Crouch:
                    SwordAnim.Play("Crouch_Lv1");
                    break;
                //case EffectType.Ranged:
                //    Main.Multiplayer.LogError("Spawning range attack effect!");
                //    break;
                case EffectType.Debla:
                    StartCoroutine(DisplayDebla());
                    break;
                    //case EffectType.Verdiales:
                    //    StartCoroutine(DisplayVerdiales());
                    //    break;
            }
        }

        private IEnumerator DisplayDebla()
        {
            ModLog.Info("Spawning debla object for " + transform.parent.name);
            if (Core.Logic.Penitent == null)
                yield break;

            GameObject debla = Instantiate(Core.Logic.Penitent.PrayerCast.lightBeamPrayer.areaPrefab, transform);
            debla.name = "Debla";
            debla.transform.localPosition = Vector3.zero;
            Destroy(debla.GetComponent<Gameplay.GameControllers.Bosses.Quirce.Attack.BossSpawnedAreaAttack>());
            Destroy(debla.transform.GetChild(0).GetChild(0).gameObject);
            debla.GetComponent<Animator>().Play("AttackLoop");
            yield return new WaitForSecondsRealtime(DEBLA_DURATION);
            Destroy(debla);
        }

        private IEnumerator DisplayVerdiales()
        {
            ModLog.Info("Spawning verdiales object for " + transform.parent.name);
            if (Core.Logic.Penitent == null)
                yield break;

            GameObject crawlerright = Instantiate(Core.Logic.Penitent.PrayerCast.crawlerBallsPrayer.projectilePrefab, transform);
            crawlerright.name = "Crawler right";
            crawlerright.transform.localPosition = Vector3.zero;
            crawlerright.GetComponentInChildren<AttackArea>().enabled = false;
            crawlerright.GetComponentInChildren<PolygonCollider2D>().enabled = false;
            crawlerright.GetComponentInChildren<StraightProjectile>().Init(crawlerright.transform.position, crawlerright.transform.position + Vector3.right, 16);
            crawlerright.transform.position += Vector3.right * 0.01f;

            GameObject crawlerleft = Instantiate(Core.Logic.Penitent.PrayerCast.crawlerBallsPrayer.projectilePrefab, transform);
            crawlerleft.name = "Crawler left";
            crawlerleft.transform.localPosition = Vector3.zero;
            crawlerleft.GetComponentInChildren<AttackArea>().enabled = false;
            crawlerleft.GetComponentInChildren<PolygonCollider2D>().enabled = false;
            crawlerleft.GetComponentInChildren<StraightProjectile>().Init(crawlerleft.transform.position, crawlerleft.transform.position + Vector3.left, 16);
            crawlerleft.transform.position += Vector3.left * 0.01f;

            yield return new WaitForSecondsRealtime(VERDIALES_DURATION);
            Destroy(crawlerright);
            Destroy(crawlerleft); // Will probably have to leave colliders active to detect walls, maybe just change layermask to exlude enemy layer ?
        }
    }
}
