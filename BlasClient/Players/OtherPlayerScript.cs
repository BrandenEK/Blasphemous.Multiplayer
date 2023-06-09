using UnityEngine;
using BlasClient.Data;
using BlasClient.PvP;
using Tools.Level.Interactables;
using Gameplay.GameControllers.Entities;
using Framework.Managers;

namespace BlasClient.Players
{
    public class OtherPlayerScript : MonoBehaviour, IDamageable
    {
        private string penitentName;
        private bool playerDead = false;

        private SpriteRenderer CharacterRenderer { get; set; }
        private Animator CharacterAnim { get; set; }

        public OtherPlayerAttackScript OtherPlayerAttack { get; private set; }

        private RuntimeAnimatorController penitentAnimatorController;

        public bool IsFacingRight => !CharacterRenderer.flipX;

        // Adds necessary components & initializes them
        public void createPenitent(string name, RuntimeAnimatorController animatorController, RuntimeAnimatorController swordAnimatorController, Material material)
        {
            penitentName = name;
            Main.Multiplayer.playerList.setPlayerSkinUpdateStatus(name, 2);

            // Rendering
            CharacterRenderer = gameObject.AddComponent<SpriteRenderer>();
            CharacterRenderer.material = material;
            CharacterRenderer.sortingLayerName = "Player";
            CharacterRenderer.enabled = false;

            // Animation
            CharacterAnim = gameObject.AddComponent<Animator>();
            CharacterAnim.runtimeAnimatorController = animatorController;
            penitentAnimatorController = animatorController;

            // Sword handler
            GameObject sword = new GameObject("Sword");
            sword.transform.SetParent(transform);
            OtherPlayerAttack = sword.AddComponent<OtherPlayerAttackScript>();
            OtherPlayerAttack.CreatePenitentAttack(swordAnimatorController);
        }

        public void updatePosition(Vector2 positon)
        {
            transform.position = positon;
        }

        public void updateAnimation(byte animation)
        {
            // If this player was previously assumed dead but now they are receiving another anim, turn them alive again
            if (playerDead)
            {
                SetDeathStatus(false);
            }

            if (animation < 240)
            {
                // Regular animation
                if (Main.Multiplayer.playerList.getPlayerSpecialAnimation(penitentName) > 0)
                {
                    // Change back to regular animations
                    CharacterAnim.runtimeAnimatorController = penitentAnimatorController;
                    Main.Multiplayer.playerList.setPlayerSpecialAnimation(penitentName, 0);
                }
                CharacterAnim.SetBool("IS_CROUCH", false);

                // Logic for ladder climbing

                // Set required parameters to keep player object in this animation
                PlayerAnimState animState = AnimationStates.animations[animation];
                for (int i = 0; i < animState.parameterNames.Length; i++)
                {
                    CharacterAnim.SetBool(animState.parameterNames[i], animState.parameterValues[i]);
                }
                CharacterAnim.Play(animState.name);
            }
            else
            {
                // Special animation
                if (playSpecialAnimation(animation))
                {
                    Main.Multiplayer.playerList.setPlayerSpecialAnimation(penitentName, animation);
                    Main.Multiplayer.Log("Playing special animation for " + name);
                }
                else
                    Main.Multiplayer.LogWarning("Failed to play special animation for " + name);
            }
        }

        public void updateDirection(bool facingDirection)
        {
            CharacterRenderer.flipX = facingDirection;
        }

        public void updateSkin(Texture2D skin)
        {
            CharacterRenderer.enabled = true;
            CharacterRenderer.material.SetTexture("_PaletteTex", skin);
        }

        // Gets the animator controller of an interactable object in the scene & plays special animation
        private bool playSpecialAnimation(byte type)
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
        public void finishSpecialAnimation()
        {
            byte currentSpecialAnimation = Main.Multiplayer.playerList.getPlayerSpecialAnimation(penitentName);
            if (currentSpecialAnimation >= 247 && currentSpecialAnimation <= 249)
            {
                // If finished entering door, disable renderer
                CharacterRenderer.enabled = false;
            }

            updateAnimation(0);
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
            //else if (!playerDead && state.normalizedTime >= 0.9f && )
            //{
            //    SetDeathStatus(true);
            //    playerDead = true;
            //}
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
                finishSpecialAnimation();
        }

        public bool BleedOnImpact() => false;
        public bool SparkOnImpact() => false;
        public Vector3 GetPosition() => transform.position;

        public void Damage(Hit hit)
        {
            Config config = Main.Multiplayer.config;
            if (!config.enablePvP || (!config.enableFriendlyFire && Main.Multiplayer.PlayerTeam == Main.Multiplayer.playerList.getPlayerTeam(penitentName)))
                return;

            Main.Multiplayer.LogError("Hit comes from " + hit.AttackingEntity.name);
            Main.Multiplayer.LogError("Hit sound id: " + hit.HitSoundId);
            AttackType attack = AttackType.Slash;

            AnimatorStateInfo penitentState = Core.Logic.Penitent.Animator.GetCurrentAnimatorStateInfo(0);
            string attackerObject = hit.AttackingEntity.name;

            if (penitentState.IsName("Combo_4"))
            {
                attack = AttackType.ComboNormal;
            }
            else if (penitentState.IsName("ComboFinisherUp"))
            {
                attack = AttackType.ComboUp;
            }
            else if (penitentState.IsName("ComboFinisherDown"))
            {
                attack = AttackType.ComboDown;
            }
            else if (penitentState.IsName("Charged Attack"))
            {
                attack = AttackType.Charged;
            }
            else if (Core.Logic.Penitent.LungeAttack.IsUsingAbility)
            {
                attack = AttackType.Lunge;
            }
            else if (Core.Logic.Penitent.VerticalAttack.IsUsingAbility)
            {
                attack = AttackType.Vertical;
            }
            else if (Core.Logic.Penitent.PrayerCast.IsUsingAbility && attackerObject == "Penitent(Clone)")
            {
                Main.Multiplayer.LogWarning("Not applying damage for prayer use!");
                return;
            }
            //else if (attackerObject == "RangeAttackProjectile(Clone)")
            //{
            //    attack = AttackType.Ranged;
            //}
            else if (attackerObject == "PenitentVerticalBeam(Clone)")
            {
                attack = AttackType.Debla;
            }
            //else if (attackerObject == "CrawlerBullet_Base(Clone)")
            //{
            //    attack = AttackType.Verdiales;
            //}
            //else if (attackerObject == "PenitentTarantoDivineLight(Clone)")
            //{
            //    attack = AttackType.Taranto;
            //}
            // Lorquiana
            //else if (attackerObject == "PR203ElmFireTrapLightning(Clone)" || attackerObject.StartsWith("ElmFireTrap")) // This might be triggered by real traps in MaH
            //{
            //    attack = AttackType.Tirana;
            //}
            //else if (attackerObject == "PrayerPoisonAreaEffect(Clone)")
            //{
            //    attack = AttackType.PoisonMist;
            //}
            //else if (attackerObject.StartsWith("PenitentShield"))
            //{
            //    attack = AttackType.Shield;
            //}
            //else if (attackerObject == "MiriamPortalPrayer(Clone)" || attackerObject.StartsWith("MiriamSpike")) // Might want to seperate these
            //{
            //    attack = AttackType.Miriam;
            //}
            //else if (attackerObject == "GuardianPrayer(Clone)")
            //{
            //    attack = AttackType.Aubade;
            //}
            // Cherubs
            // Cante Jondo

            Main.Multiplayer.LogWarning($"Sending hit {attack} to {penitentName}");
            Core.Logic.Penitent.Audio.PlaySimpleHitToEnemy();
            Main.Multiplayer.AttackManager.ShowDamageEffects(penitentName);
            Main.Multiplayer.NetworkManager.SendAttack(penitentName, attack);
        }
    }
}
