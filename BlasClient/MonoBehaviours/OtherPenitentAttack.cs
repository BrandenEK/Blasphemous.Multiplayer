using UnityEngine;
using System.Collections;
using Framework.Managers;
using BlasClient.PvP;

namespace BlasClient.MonoBehaviours
{
    public class OtherPenitentAttack : MonoBehaviour
    {
        private SpriteRenderer SwordRenderer { get; set; }
        private Animator SwordAnim { get; set; }
        private BoxCollider2D DamageArea { get; set; }

        //private GameObject DeblaAttack { get; set; }
        private float deblaLength = 1.2f;

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

            // Set up debla
            

            //DeblaAttack = new GameObject("Debla");
            //DeblaAttack.transform.SetParent(transform);
            //DeblaAttack.transform.localPosition = Vector3.zero;
            ////DeblaAttack.AddComponent<SpriteRenderer>();
            //GameObject deblaChild = new GameObject("Body", typeof(SpriteRenderer));
            //deblaChild.transform.SetParent(DeblaAttack.transform);
            //deblaChild.transform.localPosition = Vector3.zero;
            //Animator deblaAnim = DeblaAttack.AddComponent<Animator>();
            //deblaAnim.runtimeAnimatorController = Core.Logic.Penitent.PrayerCast.lightBeamPrayer.areaPrefab.GetComponent<Animator>().runtimeAnimatorController;
            //deblaAnim.SetTrigger("LOOP");
            //deblaAnim.Play("AttackLoop");
            //DeblaAttack.SetActive(false);
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
                    Main.Multiplayer.LogError("Spawning debla");
                    StartCoroutine(DisplayDebla());
                    break;
            }
        }

        private IEnumerator DisplayDebla()
        {
            Main.Multiplayer.LogError("Starting debla corroutine");
            GameObject debla = Instantiate(Core.Logic.Penitent.PrayerCast.lightBeamPrayer.areaPrefab, transform);
            debla.name = "Debla";
            debla.transform.localPosition = Vector3.zero;
            Destroy(debla.GetComponent<Gameplay.GameControllers.Bosses.Quirce.Attack.BossSpawnedAreaAttack>());
            Destroy(debla.transform.GetChild(0).GetChild(0).gameObject);
            //debla.GetComponent<Animator>().SetTrigger("LOOP");
            debla.GetComponent<Animator>().Play("AttackLoop");
            Main.Multiplayer.LogError("Created debla object");
            //debla.SetActive(false);

            //DeblaAttack.SetActive(true);
            //Main.Multiplayer.LogError("After enabling object");
            Main.Multiplayer.LogWarning(Main.displayHierarchy(transform.parent, "", 0, 6, true));
            //DeblaAttack.GetComponent<Animator>().Play("AttackLoop");
            //GameObject child = DeblaAttack.transform.GetChild(0).gameObject;
            //Main.Multiplayer.LogError((child.GetComponent<SpriteRenderer>().sprite != null) + ": " + (child.GetComponent<SpriteRenderer>().enabled && child.activeSelf).ToString());
            yield return new WaitForSecondsRealtime(deblaLength);
            Destroy(debla.gameObject);
            Main.Multiplayer.LogError("Destroyed debla object");
            //debla.SetActive(false);
        }
    }
}
