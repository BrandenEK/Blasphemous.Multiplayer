using Blasphemous.Multiplayer.Client.Data;
using Blasphemous.Multiplayer.Client.PvP;
using Framework.Managers;
using Gameplay.GameControllers.Entities;
using System.Linq;
using Tools.Level.Interactables;
using UnityEngine;

namespace Blasphemous.Multiplayer.Client.Players
{
    public class OtherPlayerScript : MonoBehaviour, IDamageable
    {
        private PlayerStatus playerStatus;
        private bool playerDead = false;

        private SpriteRenderer CharacterRenderer { get; set; }
        private Animator CharacterAnim { get; set; }

        public OtherPlayerAttackScript OtherPlayerAttack { get; private set; }

        public bool IsFacingRight => !CharacterRenderer.flipX;

        // Adds necessary components & initializes them
        public void SetupPlayer(PlayerStatus status)
        {
            playerStatus = status;
            playerStatus.SkinUpdateStatus = PlayerStatus.SkinStatus.NoUpdate;

            // Rendering
            CharacterRenderer = gameObject.AddComponent<SpriteRenderer>();
            CharacterRenderer.material = Main.Multiplayer.PlayerMaterial;
            CharacterRenderer.sortingLayerName = "Player";
            CharacterRenderer.enabled = false;

            // Animation
            CharacterAnim = gameObject.AddComponent<Animator>();
            CharacterAnim.runtimeAnimatorController = Main.Multiplayer.PlayerAnimator;

            // Sword handler
            GameObject sword = new GameObject("Sword");
            sword.transform.SetParent(transform);
            OtherPlayerAttack = sword.AddComponent<OtherPlayerAttackScript>();
            OtherPlayerAttack.SetupAttack();
        }

        public Vector2 CurrentPosition
        {
            set
            {
                transform.position = value;
            }
        }

        public byte CurrentAnimation
        {
            set
            {
                // If this player was previously assumed dead but now they are receiving another anim, turn them alive again
                if (playerDead)
                {
                    SetDeathStatus(false);
                }

                if (value < 240)
                {
                    // Regular animation
                    if (playerStatus.SpecialAnimation > 0)
                    {
                        // Change back to regular animations
                        CharacterAnim.runtimeAnimatorController = Main.Multiplayer.PlayerAnimator;
                        playerStatus.SpecialAnimation = 0;
                    }
                    CharacterAnim.SetBool("IS_CROUCH", false);

                    // Logic for ladder climbing

                    // Set required parameters to keep player object in this animation
                    PlayerAnimState animState = AnimationStates.animations[value];
                    for (int i = 0; i < animState.parameterNames.Length; i++)
                    {
                        CharacterAnim.SetBool(animState.parameterNames[i], animState.parameterValues[i]);
                    }
                    CharacterAnim.Play(animState.name);
                }
                else
                {
                    // Special animation
                    if (PlaySpecialAnimation(value))
                    {
                        playerStatus.SpecialAnimation = value;
                        Main.Multiplayer.Log("Playing special animation for " + name);
                    }
                    else
                        Main.Multiplayer.LogWarning("Failed to play special animation for " + name);
                }
            }
        }

        public bool CurrentDirection
        {
            set
            {
                CharacterRenderer.flipX = value;
            }
        }

        public void ApplySkinTexture()
        {
            CharacterRenderer.enabled = true;
            CharacterRenderer.material.SetTexture("_PaletteTex", playerStatus.SkinTexture);
        }

        // Gets the animator controller of an interactable object in the scene & plays special animation
        private bool PlaySpecialAnimation(byte type)
        {
            if (type == 240 || type == 241 || type == 242)
            {
                // Prie Dieu
                PrieDieu priedieu = FindObjectOfType<PrieDieu>();
                if (priedieu == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = priedieu.transform.GetChild(4).GetComponent<Animator>().runtimeAnimatorController;
                if (type == 240)
                {
                    CharacterAnim.SetTrigger("ACTIVATION");
                }
                else if (type == 241)
                {
                    CharacterAnim.SetTrigger("KNEE_START");
                }
                else
                {
                    CharacterAnim.Play("Stand Up");
                }
            }
            else if (type == 243 || type == 244)
            {
                // Collectible item
                CollectibleItem item = FindObjectOfType<CollectibleItem>();
                if (item == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = item.transform.GetChild(1).GetComponent<Animator>().runtimeAnimatorController;
                CharacterAnim.Play(type == 244 ? "Floor Collection" : "Halfheight Collection");
            }
            else if (type == 245)
            {
                // Chest
                Chest chest = FindObjectOfType<Chest>();
                if (chest == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = chest.transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController;
                CharacterAnim.SetTrigger("USED");
            }
            else if (type == 246)
            {
                // Lever
                Lever lever = FindObjectOfType<Lever>();
                if (lever == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = lever.transform.GetChild(2).GetComponent<Animator>().runtimeAnimatorController;
                CharacterAnim.SetTrigger("DOWN");
            }
            else if (type == 247 || type == 248 || type == 249)
            {
                // Door
                Door door = FindObjectOfType<Door>();
                if (door == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = door.transform.GetChild(3).GetComponent<Animator>().runtimeAnimatorController;
                if (type == 247)
                {
                    CharacterAnim.SetTrigger("OPEN_ENTER");
                }
                else if (type == 248)
                {
                    CharacterAnim.SetTrigger("CLOSED_ENTER");
                }
                else
                {
                    CharacterAnim.SetTrigger("KEY_ENTER");
                }
            }
            else if (type == 250 || type == 251)
            {
                // Fake penitent
                GameObject logic = GameObject.Find("LOGIC");
                if (logic == null)
                    return false;

                CharacterAnim.runtimeAnimatorController = logic.transform.GetChild(3).GetComponent<Animator>().runtimeAnimatorController;
                CharacterAnim.Play(type == 250 ? "FakePenitent laydown" : "FakePenitent gettingUp");
            }
            else
            {
                return false;
            }

            return true;
        }

        // Finishes playing a special animation and returns to idle
        public void FinishSpecialAnimation()
        {
            if (playerStatus.SpecialAnimation >= 247 && playerStatus.SpecialAnimation <= 249)
            {
                // If finished entering door, disable renderer
                CharacterRenderer.enabled = false;
            }

            CurrentAnimation = 0;
        }

        // Check if death animation is playing
        private void Update()
        {
            AnimatorStateInfo state = CharacterAnim.GetCurrentAnimatorStateInfo(0);
            if (!playerDead && state.normalizedTime >= 0.95f && (state.IsName("Death") || state.IsName("Death Spike") || state.IsName("Death Fall") || state.IsName("Grounding Over")))
            {
                SetDeathStatus(true);
                playerDead = true;
            }
            else
            {
                playerDead = false;
            }
        }

        private void SetDeathStatus(bool dead)
        {
            CharacterAnim.enabled = !dead;
            OtherPlayerAttack.SetHitboxStatus(!dead);
        }

        // Finish a special animation when the event is received
        public void LaunchEvent(string eventName)
        {
            if (eventName == "INTERACTION_END")
                FinishSpecialAnimation();
        }

        public bool BleedOnImpact() => false;
        public bool SparkOnImpact() => false;
        public Vector3 GetPosition() => transform.position;

        public void Damage(Hit hit)
        {
            Config config = Main.Multiplayer.config;
            if (!config.enablePvP || (!config.enableFriendlyFire && Main.Multiplayer.PlayerTeam == playerStatus.Team))
                return;

            AttackType attack = CalculateAttack(hit);
            if (attack == AttackType.NoDamage || _blockedAttacks.Contains(attack))
                return;

            // Calculate damage amount based on attack type, sword level, and equipment
            float damageAmount = Main.Multiplayer.DamageCalculator.CalculateOffense(attack);

            Main.Multiplayer.LogWarning($"Sending hit {attack} to {playerStatus.Name} ({damageAmount} damage)");
            Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
            Main.Multiplayer.AttackManager.ShowDamageEffects(playerStatus.Name);
            Main.Multiplayer.NetworkManager.SendAttack(playerStatus.Name, attack, (byte)damageAmount);
        }

        private AttackType CalculateAttack(Hit hit)
        {
            string attacker = hit.AttackingEntity.name;
            string animation = AnimationStates.animations
                .Select(x => x.name)
                .FirstOrDefault(x => Core.Logic.Penitent.Animator.GetCurrentAnimatorStateInfo(0).IsName(x)) ?? "Unknown";
            DamageArea.DamageType type = hit.DamageType;
            DamageArea.DamageElement element = hit.DamageElement;
            float force = hit.Force;

            Main.Multiplayer.LogError($"{playerStatus.Name} received damage [{attacker},{animation},{type},{element},{force}]");
            //Main.Multiplayer.LogError("Hit sound id: " + hit.HitSoundId);

            if (attacker == "Penitent(Clone)")
            {
                if (animation == "Combo_4")
                {
                    return AttackType.ComboNormal;
                }
                if (animation == "ComboFinisherUp")
                {
                    return AttackType.ComboUp;
                }
                if (animation == "ComboFinisherDown")
                {
                    return AttackType.ComboDown;
                }
                if (animation == "Charged Attack")
                {
                    return hit.Force == 0 ? AttackType.ChargedProjectile : AttackType.Charged;
                }
                if (animation == "AuraTransform")
                {
                    return force == 0 ? AttackType.PrayerHit : AttackType.Lorquiana;
                }
                if (Core.Logic.Penitent.LungeAttack.IsUsingAbility)
                {
                    return AttackType.Lunge;
                }
                if (Core.Logic.Penitent.VerticalAttack.IsUsingAbility)
                {
                    return AttackType.Vertical;
                }

                return force == 0 ? AttackType.RangedExplosion : AttackType.Slash;
            }
            
            if (attacker == "RangeAttackProjectile(Clone)")
            {
                return AttackType.Ranged;
            }
            if (attacker == "PenitentCloisteredGemBullet(Clone)")
            {
                return AttackType.Gem;
            }
            if (attacker == "PR203ElmFireTrapLightning(Clone)" || attacker.StartsWith("ElmFireTrap")) // This might be triggered by real traps in MaH
            {
                return AttackType.Tirana;
            }
            if (attacker == "PenitentVerticalBeam(Clone)")
            {
                return AttackType.Debla;
            }
            if (attacker == "PenitentTarantoDivineLight(Clone)")
            {
                return AttackType.Taranto;
            }
            if (attacker == "CrawlerBullet_Base(Clone)")
            {
                return AttackType.Verdiales;
            }
            if (attacker == "PrayerPoisonAreaEffect(Clone)")
            {
                return AttackType.PoisonMist;
            }
            if (attacker.StartsWith("PenitentShield"))
            {
                return AttackType.Shield;
            }
            if (attacker == "MiriamPortalPrayer(Clone)")
            {
                return AttackType.Miriam;
            }
            if (attacker.StartsWith("MiriamSpike"))
            {
                return AttackType.MiriamSpike;
            }
            if (attacker == "GuardianPrayer(Clone)")
            {
                return AttackType.Aubade;
            }

            // Cherubs
            // Cante Jondo

            return AttackType.NoDamage;
        }

        private readonly AttackType[] _blockedAttacks =
        {
            AttackType.Ranged,
            AttackType.ChargedProjectile,
            AttackType.RangedExplosion,
            AttackType.Gem,
            AttackType.PrayerHit,
            AttackType.Taranto,
            AttackType.Verdiales,
            AttackType.Lorquiana,
            AttackType.Tirana,
            AttackType.PoisonMist,
            AttackType.Shield,
            AttackType.Miriam,
            AttackType.Aubade,
            AttackType.Cherubs,
            AttackType.CanteJondo,
        };
    }
}
