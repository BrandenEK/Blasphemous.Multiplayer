using UnityEngine;
using System.Collections;
using Framework.Managers;
using BlasClient.PvP;
using Gameplay.GameControllers.Entities;
using Gameplay.GameControllers.Enemies.Projectiles;

namespace BlasClient.MonoBehaviours
{
    public class OtherPenitentAttack : MonoBehaviour
    {
        private const float DEBLA_DURATION = 1.2f;

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
                case EffectType.Ranged:
                    Main.Multiplayer.LogError("Spawning range attack effect!");
                    break;
                case EffectType.Debla:
                    StartCoroutine(DisplayDebla());
                    break;
                case EffectType.Verdiales:
                    StartCoroutine(DisplayVerdiales());
                    break;
            }
        }

        private IEnumerator DisplayDebla()
        {
            Main.Multiplayer.Log("Spawning debla object for " + transform.parent.name);
            if (Core.Logic.Penitent == null)
                yield break;

            GameObject debla = Instantiate(Core.Logic.Penitent.PrayerCast.lightBeamPrayer.areaPrefab, transform);
            debla.name = "Debla";
            debla.transform.localPosition = Vector3.zero;
            Destroy(debla.GetComponent<Gameplay.GameControllers.Bosses.Quirce.Attack.BossSpawnedAreaAttack>());
            Destroy(debla.transform.GetChild(0).GetChild(0).gameObject);
            debla.GetComponent<Animator>().Play("AttackLoop");
            yield return new WaitForSecondsRealtime(DEBLA_DURATION);
            Destroy(debla.gameObject);
        }

        private IEnumerator DisplayVerdiales()
        {
            Main.Multiplayer.Log("Spawning verdiales object for " + transform.parent.name);
            if (Core.Logic.Penitent == null)
                yield break;

            GameObject crawler = Instantiate(Core.Logic.Penitent.PrayerCast.crawlerBallsPrayer.projectilePrefab, transform);
            crawler.name = "Crawler right";
            crawler.transform.localPosition = Vector3.zero;
            crawler.GetComponentInChildren<AttackArea>().enabled = false;
            crawler.GetComponentInChildren<StraightProjectile>().Init(crawler.transform.position, crawler.transform.position + Vector3.right, 5); // Need to change speed
            crawler.transform.position += Vector3.right * 0.01f;

            crawler = Instantiate(Core.Logic.Penitent.PrayerCast.crawlerBallsPrayer.projectilePrefab, transform);
            crawler.name = "Crawler left";
            crawler.transform.localPosition = Vector3.zero;
            crawler.GetComponentInChildren<AttackArea>().enabled = false;
            crawler.GetComponentInChildren<StraightProjectile>().Init(crawler.transform.position, crawler.transform.position + Vector3.left, 5); // Need to change speed
            crawler.transform.position += Vector3.left * 0.01f;

            // Maybe have to wait for some time and destroy them ?
        }
    }
}
